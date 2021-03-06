﻿using System;
using System.Collections.Generic;
using System.Reactive;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Caliburn.Micro;
using SBoard.ApplicationModes;
using SBoard.Core.Commands;
using SBoard.Core.Commands.Helpdesks;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Exceptions;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
using SBoard.Core.Queries.Helpdesks;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Core.Services.Scripts;
using SBoard.Strings;
using SBoard.Views.Dashboard;
using SBoard.Views.HelpdeskList;
using SBoard.Views.Login;
using SBoard.Views.NewHelpdeskGroup;
using UwCore.Application;
using UwCore.Services.ApplicationState;

namespace SBoard
{
    sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            base.Configure();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }

        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(LoginViewModel);
            yield return typeof(HelpdeskListViewModel);
            yield return typeof(NewHelpdeskGroupViewModel);
            yield return typeof(HelpdeskListItemViewModel);
            yield return typeof(DashboardViewModel);
        }

        public override void ConfigureContainer(WinRTContainer container)
        {
            container
                .PerRequest<LoggedOutApplicationMode>()
                .PerRequest<LoggedInApplicationMode>();

            container
                .Singleton<ICentronService, CentronService>()
                .Singleton<IHelpdeskGroupsService, HelpdeskGroupsService>()
                .Singleton<IQueryExecutor, QueryExecutor>()
                .Singleton<IQueryCache, QueryCache>()
                .Singleton<ICommandQueue, CommandQueue>()
                .Singleton<IScriptEngine, ScriptEngine>();
            
            container
                .PerRequest<IQueryHandler<HelpdeskGroupQuery, IList<HelpdeskPreview>>, HelpdeskGroupQueryHandler>()
                .PerRequest<IQueryHandler<SearchCustomersQuery, IList<CustomerPreview>>, SearchCustomersQueryHandler>()
                .PerRequest<IQueryHandler<HelpdeskTypesQuery, IList<HelpdeskType>>, HelpdeskTypesQueryHandler>()
                .PerRequest<IQueryHandler<HelpdeskStatesQuery, IList<HelpdeskState>>, HelpdeskStatesQueryHandler>();

            container
                .PerRequest<ICommandHandler<ChangeHelpdeskStateCommand, Unit>, ChangeHelpdeskStateCommandHandler>();

            var scriptEngine = container.GetInstance<IScriptEngine>();
            scriptEngine.AddGlobalMethod("contains", (Func<string, string, bool>)((s1, s2) => (s1 ?? string.Empty).IndexOf(s2, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public override string GetErrorTitle() => SBoardResources.Get("Errors.Title");

        public override string GetErrorMessage() => SBoardResources.Get("Errors.Message");

        public override Type GetCommonExceptionType() => typeof(SBoardException);
        
        public override ApplicationMode GetCurrentMode()
        {
            var state = IoC.Get<IApplicationStateService>();
            string webServiceAddress = state.GetWebServiceAddress();
            string username = state.GetUsername();
            string password = state.GetPassword();

            if (string.IsNullOrWhiteSpace(webServiceAddress) == false &&
                string.IsNullOrWhiteSpace(username) == false &&
                string.IsNullOrWhiteSpace(password) == false)
            {
                return IoC.Get<LoggedInApplicationMode>();
            }
            else
            {
                return IoC.Get<LoggedOutApplicationMode>();
            }
        }
    }
}
