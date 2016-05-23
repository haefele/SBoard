using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Exceptions;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Core.Services.Scripts;

namespace SBoard.Core.Queries.Helpdesks
{
    public class HelpdeskGroupQuery : IQuery<IList<HelpdeskPreview>>, IEquatable<HelpdeskGroupQuery>
    {
        public string HelpdeskListId { get; }

        public HelpdeskGroupQuery(string helpdeskListId)
        {
            this.HelpdeskListId = helpdeskListId;
        }

        #region Equality
        public bool Equals(HelpdeskGroupQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this.HelpdeskListId, other.HelpdeskListId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HelpdeskGroupQuery) obj);
        }

        public override int GetHashCode()
        {
            return (this.HelpdeskListId != null ? this.HelpdeskListId.GetHashCode() : 0);
        }

        public static bool operator ==(HelpdeskGroupQuery left, HelpdeskGroupQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskGroupQuery left, HelpdeskGroupQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class HelpdeskGroupQueryHandler : IQueryHandler<HelpdeskGroupQuery, IList<HelpdeskPreview>>
    {
        private readonly ICentronService _centronService;
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IScriptEngine _scriptEngine;

        public HelpdeskGroupQueryHandler(ICentronService centronService, IHelpdeskGroupsService helpdeskGroupsService, IScriptEngine scriptEngine)
        {
            this._centronService = centronService;
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._scriptEngine = scriptEngine;
        }

        public async Task<IList<HelpdeskPreview>> ExecuteAsync(HelpdeskGroupQuery query)
        {
            var helpdeskLists = await this._helpdeskGroupsService.GetHelpdeskGroupsAsync();
            var helpdeskList = helpdeskLists.First(f => f.Id == query.HelpdeskListId);

            var helpdesks = await this._centronService.GetHelpdesksAsync(
                helpdeskList.CustomerI3D, 
                helpdeskList.OnlyOwn,
                helpdeskList.HelpdeskStateI3D);

            if (helpdeskList.HelpdeskTypeI3D.HasValue)
            {
                helpdesks = helpdesks
                    .Where(f => f.TypeI3D == helpdeskList.HelpdeskTypeI3D.Value)
                    .ToList();
            }

            if (string.IsNullOrWhiteSpace(helpdeskList.FilterScript) == false)
            {
                var script = this._scriptEngine.CreateFor("return " + helpdeskList.FilterScript);

                helpdesks = helpdesks
                    .Where(f =>
                    {
                        script.AddData("nummer", f.Number);
                        script.AddData("kategorie", f.CategoryCaption);
                        script.AddData("unterkategorie1", f.SubCategory1Caption);
                        script.AddData("unterkategorie2", f.SubCategory2Caption);
                        script.AddData("status", f.StatusCaption);
                        script.AddData("priorität", f.PriorityCaption);
                        script.AddData("typ", f.TypeCaption);

                        try
                        {
                            return script.Execute().AsBoolean();
                        }
                        catch (InvalidScriptException)
                        {
                            return true;
                        }
                    })
                    .ToList();
            }
            
            return helpdesks;
        }
    }
}