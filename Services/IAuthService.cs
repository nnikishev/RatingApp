using RatingApp.Models;

namespace RatingApp.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string username, string password);
        Task<bool> LogoutAsync();
        Task<bool> ValidateTokenAsync();
        AuthInfo GetAuthInfo();
        void SaveAuthInfo(AuthInfo authInfo);
        void ClearAuthInfo();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://10.0.3.11:8084/api/auth";
        private const string AuthStorageKey = "auth_info";

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var json = System.Text.Json.JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);
                    
                    if (loginResponse != null)
                    {
                        // Сохраняем информацию об авторизации
                        var authInfo = new AuthInfo
                        {
                            Token = loginResponse.Token,
                            RefreshToken = loginResponse.RefreshToken,
                            ExpiresAt = loginResponse.ExpiresAt,
                            UserId = loginResponse.UserId,
                            Username = loginResponse.Username
                        };
                        SaveAuthInfo(authInfo);
                    }
                    
                    return loginResponse;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"LOGIN_ERROR: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LOGIN_EXCEPTION: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var authInfo = GetAuthInfo();
                if (!string.IsNullOrEmpty(authInfo.Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authInfo.Token);
                    
                    var response = await _httpClient.PostAsync($"{BaseUrl}/logout", null);
                    // Even if logout fails on server, we clear local auth
                }
                
                ClearAuthInfo();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LOGOUT_EXCEPTION: {ex.Message}");
                ClearAuthInfo();
                return true;
            }
        }

        public async Task<bool> ValidateTokenAsync()
        {
            var authInfo = GetAuthInfo();
            return authInfo.IsAuthenticated;
        }

        public AuthInfo GetAuthInfo()
        {
            try
            {
                if (Preferences.ContainsKey(AuthStorageKey))
                {
                    var json = Preferences.Get(AuthStorageKey, string.Empty);
                    if (!string.IsNullOrEmpty(json))
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<AuthInfo>(json) ?? new AuthInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GET_AUTH_INFO_ERROR: {ex.Message}");
            }
            
            return new AuthInfo();
        }

        public void SaveAuthInfo(AuthInfo authInfo)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(authInfo);
                Preferences.Set(AuthStorageKey, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SAVE_AUTH_INFO_ERROR: {ex.Message}");
            }
        }

        public void ClearAuthInfo()
        {
            try
            {
                Preferences.Remove(AuthStorageKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CLEAR_AUTH_INFO_ERROR: {ex.Message}");
            }
        }
    }
}