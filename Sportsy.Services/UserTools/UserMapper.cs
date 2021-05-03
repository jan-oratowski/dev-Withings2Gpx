using Sportsy.Data.Models;
using Sportsy.WebClient.Shared.Models;

namespace Sportsy.Services.UserTools
{
    public static class UserMapper
    {
        public static UserBase ToUserBase(this User user) =>
            new UserBase
            {
                Id = user.Id,
                Name = user.Name,
            };
    }
}
