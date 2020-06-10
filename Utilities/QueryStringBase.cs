using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Utilities
{
    public abstract class QueryStringBase
    {
        public string url { get; set; }
        public string GetDynamicURLWithQueryString()
        {            
            return QueryHelpers.AddQueryString(
                url, 
                this.GetType().GetProperties()
                .Where(x => x.Name != "url" && x.GetValue(this) != null)
                .Select(x => 
                new
                {
                    key = x.Name,
                    value = x.GetValue(this).ToString()
                }).ToDictionary(x => x.key, x => x.value));            
        }
    }
}
