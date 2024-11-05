using budget.core.Services.Interfaces;

namespace budget.webapi.Factories {
  public class UserSettingsServiceFactory {
    private readonly IServiceProvider _serviceProvider;

    public UserSettingsServiceFactory(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;
    }

    public IUserSettingsService Create() {
      return _serviceProvider.GetRequiredService<IUserSettingsService>();
    }
  }
}
