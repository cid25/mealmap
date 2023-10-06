using System.Text.Json.Serialization;

namespace Mealmap.Api.DataTransferObjects
{
    public class PaginatedDTO<T>
        where T : class
    {
        /// <summary>
        /// The items for this chunk of the request's resultset.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The Url to fetch the next set of result items.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Uri? Next { get; set; }
    }
}
