﻿namespace Dragonfly.Umbraco7Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Core;
    using Umbraco.Core.Models;

    public static class Relations
    {
        private const string ThisClassName = "Dragonfly.Umbraco7Helpers.Relations";

        /// <summary>
        /// Get a list of related node Ids with duplicates removed (esp. for a bi-directional relation)
        /// </summary>
        /// <param name="LookupNodeId">Id of node to get relations for</param>
        /// <param name="RelationAlias">If blank will check all relations</param>
        /// <returns></returns>
        public static IEnumerable<int> GetDistinctRelatedNodeIds(int LookupNodeId, string RelationAlias = "")
        {
            var relationsService = ApplicationContext.Current.Services.RelationService;
            var collectedIds = new List<int>();

            IEnumerable<IRelation> relations = null;

            //var test = relationsService.GetAllRelations();

            if (!string.IsNullOrEmpty(RelationAlias))
            {
                relations = relationsService.GetByParentOrChildId(LookupNodeId, RelationAlias);
            }
            else
            {
                relations = relationsService.GetByParentOrChildId(LookupNodeId);
            }

            foreach (var pair in relations)
            {
                if (pair.ParentId == LookupNodeId)
                {
                    collectedIds.Add(pair.ChildId);
                }

                if (pair.ChildId == LookupNodeId)
                {
                    collectedIds.Add(pair.ParentId);
                }
            }

            return collectedIds.Distinct();
        }
    }
}
