using System.Text.Json.Serialization;

namespace Mealmap.Api.DataTransferObjects
{
    public class PaginatedDTO<T>
        where T : class
    {
        /// <summary>
        /// The items for this chunk of the request's resultset.
        /// </summary>
        public IEnumerable<T> Items { get; init; }

        /// <summary>
        /// The Url to fetch the next set of result items.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Uri? Next { get; init; }

        /// <summary>
        /// The Url to fetch the previous set of result items.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Uri? Previous { get; init; }

        public PaginatedDTO(IEnumerable<T> items)
        {
            Items = items;
        }
    }
}
