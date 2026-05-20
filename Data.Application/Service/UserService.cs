namespace Data.Application;

public interface IUserService
{
    Task<string> GetFormattedUserNameAsync(int id);
    Task<string> GetFormattedUserNameQuick(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GetFormattedUserNameAsync(int id)
    {
        var name = await _userRepository.GetUserNameByIdAsync(id);

        await Task.Delay(15000);

        if (string.IsNullOrEmpty(name))
        {
            return $"{id}번 유저를 찾을 수 없습니다.";
        }

        return $"[조회 성공] {name}님 환영합니다!";
    }

    public async Task<string> GetFormattedUserNameQuick(int id)
    {
        var name = await _userRepository.GetUserNameByIdAsync(id);

        if (string.IsNullOrEmpty(name))
        {
            return $"{id}번 유저를 찾을 수 없습니다.";
        }

        return $"[조회 성공] {name}님 환영합니다!";
    }
}