using Microsoft.AspNetCore.Mvc;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class InvolvedCompanies : DataGetter<InvolvedCompany>
    {
        public InvolvedCompanies(IgdbApi igdbApi) : base(igdbApi, "involved_companies")
        {
        }

        public async Task<ExpandedInvolvedCompany?> GetExpanded(ulong companyId)
        {
            var involvedCompany = await igdbApi.GetItem(companyId, endpointPath, Collection);
            if (involvedCompany == null)
            {
                return null;
            }

            var expandedCompany = involvedCompany.ToExpanded();
            expandedCompany.company = await igdbApi.Companies.Get(involvedCompany.company);
            return expandedCompany;
        }

        public async Task<List<ExpandedInvolvedCompany>?> GetExpanded(List<ulong>? objectIds)
        {
            if (!objectIds.HasItems())
            {
                return null;
            }

            var involvedCompanies = await igdbApi.GetItem(objectIds, endpointPath, Collection);
            if (!involvedCompanies.HasItems())
            {
                return null;
            }

            var expandedCompanies = new List<ExpandedInvolvedCompany>();
            foreach (var company in involvedCompanies)
            {
                var expandedCompany = company.ToExpanded();
                expandedCompanies.Add(expandedCompany);
            }

            var realCompanies = await igdbApi.Companies.Get(involvedCompanies.Select(a => a.company).Distinct().ToList());
            for (int i = 0; i < involvedCompanies.Count; i++)
            {
                expandedCompanies[i].company = realCompanies?.FirstOrDefault(a => a.id == involvedCompanies[i].company);
            }

            return expandedCompanies;
        }
    }
}
