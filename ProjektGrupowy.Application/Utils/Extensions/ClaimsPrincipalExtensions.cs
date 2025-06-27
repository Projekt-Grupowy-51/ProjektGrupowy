using System.Security.Claims;

namespace ProjektGrupowy.Domain.Utils.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal object.</param>
        /// <returns>The user ID as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when user ID claim is not found.</exception>
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found.");
            }
            
            return userIdClaim.Value;
        }

        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal and parses it as an integer.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal object.</param>
        /// <returns>The user ID as an integer.</returns>
        /// <exception cref="InvalidOperationException">Thrown when user ID claim is not found or cannot be parsed as integer.</exception>
        public static int GetUserIdAsInt(this ClaimsPrincipal principal)
        {
            var userIdString = principal.GetUserId(); // This will throw if ID not found
            
            if (!int.TryParse(userIdString, out var userId))
            {
                throw new InvalidOperationException("User ID is not a valid integer.");
            }
            
            return userId;
        }
    }
}
