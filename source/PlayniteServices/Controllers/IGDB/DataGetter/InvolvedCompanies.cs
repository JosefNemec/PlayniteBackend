using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public async Task<ExpandedInvolvedCompany> GetExpanded(ulong companyId)
        {
            var involvedCompany = await igdbApi.GetItem(companyId, endpointPath, Collection);
            var expandedCompany = new ExpandedInvolvedCompany();
            involvedCompany.CopyProperties(expandedCompany, false, new List<string>()
            {
                nameof(InvolvedCompany.company)
            });

            expandedCompany.company = await igdbApi.Companies.Get(involvedCompany.company);
            return expandedCompany;
        }

        public async Task<List<ExpandedInvolvedCompany>> GetExpanded(List<ulong> objectIds)
        {
            var involvedCompanies = await igdbApi.GetItem(objectIds, endpointPath, Collection);
            var expandedCompanies = new List<ExpandedInvolvedCompany>();
            foreach (var company in involvedCompanies)
            {
                var expandedCompany = new ExpandedInvolvedCompany();
                company.CopyProperties(expandedCompany, false, new List<string>()
                {
                    nameof(InvolvedCompany.company)
                });

                expandedCompanies.Add(expandedCompany);
            }

            var realCompanies = await igdbApi.Companies.Get(involvedCompanies.Select(a => a.company).Distinct().ToList());
            for (int i = 0; i < involvedCompanies.Count; i++)
            {
                expandedCompanies[i].company = realCompanies.FirstOrDefault(a => a.id == involvedCompanies[i].company);
            }

            return expandedCompanies;
        }
    }
}
