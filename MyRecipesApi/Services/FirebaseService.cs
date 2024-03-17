using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using Microsoft.Extensions.Options;

namespace MyRecipesApi.Services
{
    public class FirebaseService
    {
        private readonly FirebaseAuthClient _client;
        private UserCredential? _userCredential;

        private readonly IOptions<FirebaseSettings> _settings;

        public FirebaseService(IOptions<FirebaseSettings> settings)
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = settings.Value.ApiKey,
                AuthDomain = settings.Value.AuthDomain,
                Providers =
                [
                    new EmailProvider()
                ],
            };
            _client = new FirebaseAuthClient(config);
            _settings = settings;
        }

        public async Task<String?> UploadImage(IFormFile formFile) 
        {
            if (formFile.ContentType != "image/jpeg" && formFile.ContentType != "image/png")
            {
                return null;
            }
            _userCredential ??= await SignIn();

            var firebaseStorage = new FirebaseStorage(
                _settings.Value.StorageBucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(_userCredential.User.Credential.IdToken),
                    ThrowOnCancel = true
                });

            var task = firebaseStorage
                .Child("images")
                .Child("recipe")
                .Child(Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName))
                .PutAsync(formFile.OpenReadStream());

            var downloadUrl = await task;
            Console.WriteLine("Download Url: " + downloadUrl);
                
            return downloadUrl;
        }

        private async Task<UserCredential?> SignIn()
        {
            UserCredential? userCredential = null;
            try
            {
                userCredential = await _client.SignInWithEmailAndPasswordAsync(_settings.Value.LoginEmail, _settings.Value.LoginPassword);
                Console.WriteLine("Successfully signed in to Firebase");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to sign in to Firebase", ex);
            }
            return userCredential;
        }
    }
}
