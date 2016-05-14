using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Markup;

namespace SBoard.Behaviors
{
    public class ContextMenuTriggerBehaviorData
    {
        public ContextMenuTriggerBehaviorData(UIElement control, Point position, object data)
        {
            this.Control = control;
            this.Position = position;
            this.Data = data;
        }

        public UIElement Control { get; }
        public Point Position { get; }
        public object Data { get; }
    }

    [ContentProperty(Name = nameof(Actions))]
    public class ContextMenuTriggerBehavior : Behavior<UIElement>
    {
        private bool _isShiftPressed;
        private bool _isPointerPressed;

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            nameof(Actions),
            typeof(ActionCollection),
            typeof(ContextMenuTriggerBehavior),
            new PropertyMetadata(default(ActionCollection)));

        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection)this.GetValue(ActionsProperty);

                if (actions == null)
                {
                    actions = new ActionCollection();
                    this.SetValue(ActionsProperty, actions);
                }

                return actions;
            }
        }

        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
            nameof(DataType), 
            typeof(Type), 
            typeof(ContextMenuTriggerBehavior), 
            new PropertyMetadata(default(Type)));

        public Type DataType
        {
            get { return (Type) this.GetValue(DataTypeProperty); }
            set { this.SetValue(DataTypeProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.KeyDown += this.OnKeyDown;
            this.AssociatedObject.KeyUp += this.OnKeyUp;
            this.AssociatedObject.Holding += this.OnHolding;
            this.AssociatedObject.PointerPressed += this.OnPointerPressed;
            this.AssociatedObject.RightTapped += this.OnRightTapped;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            
            this.AssociatedObject.KeyDown -= this.OnKeyDown;
            this.AssociatedObject.KeyUp -= this.OnKeyUp;
            this.AssociatedObject.Holding -= this.OnHolding;
            this.AssociatedObject.PointerPressed -= this.OnPointerPressed;
            this.AssociatedObject.RightTapped -= this.OnRightTapped;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Handle Shift+F10
            // Handle MenuKey
            if (e.Key == VirtualKey.Shift)
            {
                this._isShiftPressed = true;
            }

            // Shift+F10
            else if (this._isShiftPressed && e.Key == VirtualKey.F10)
            {
                this.ShowContextMenu(FocusManager.GetFocusedElement() as UIElement, new Point(0, 0));
                e.Handled = true;
            }

            // The 'Menu' key next to Right Ctrl on most keyboards
            else if (e.Key == VirtualKey.Application)
            {
                this.ShowContextMenu(FocusManager.GetFocusedElement() as UIElement, new Point(0, 0));
                e.Handled = true;
            }
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                this._isShiftPressed = false;
            }
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            // Responding to HoldingState.Started will show a context menu while your finger is still down, while 
            // HoldingState.Completed will wait until the user has removed their finger. 
            if (e.HoldingState == HoldingState.Completed)
            {
                var position = e.GetPosition(null);
                this.ShowContextMenu(FocusManager.GetFocusedElement() as UIElement, position);
                e.Handled = true;

                // This, combined with a check in OnRightTapped prevents the firing of RightTapped from
                // launching another context menu
                this._isPointerPressed = false;

                // This prevents any scrollviewers from continuing to pan once the context menu is displayed.  
                // Ideally, you should find the ListViewItem itself and only CancelDirectMinpulations on that item.  
                var toCancel = VisualTreeHelper.FindElementsInHostCoordinates(position, this.AssociatedObject);
                foreach (var item in toCancel)
                {
                    item.CancelDirectManipulations();
                }
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this._isPointerPressed = true;
        }

        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (this._isPointerPressed)
            {
                this.ShowContextMenu(FocusManager.GetFocusedElement() as UIElement, e.GetPosition(FocusManager.GetFocusedElement() as UIElement));
                e.Handled = true;
            }
        }

        private object FindData(UIElement parent, Point position)
        {
            if (this.DataType == null)
                return null;

            var elements = VisualTreeHelper.FindElementsInHostCoordinates(position, parent);
            foreach (var element in elements.Concat(new List<UIElement> { parent }))
            {
                if (this.DataType.IsInstanceOfType(element))
                {
                    return element;
                }
                if (element is FrameworkElement && this.DataType.IsInstanceOfType(((FrameworkElement) element).DataContext))
                {
                    return ((FrameworkElement) element).DataContext;
                }
                if (element is ContentControl && this.DataType.IsInstanceOfType(((ContentControl) element).Content))
                {
                    return ((ContentControl) element).Content;
                }
            }

            return null;
        }

        private void ShowContextMenu(UIElement target, Point offset)
        {
            var data = this.FindData(target ?? this.AssociatedObject, offset);
            Interaction.ExecuteActions(this, this.Actions, new ContextMenuTriggerBehaviorData(target, offset, data));
        }
    }
}