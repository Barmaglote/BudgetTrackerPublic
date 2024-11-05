namespace budget.webapi.Services.Interfaces {
  public interface IMonitoringService {
    bool Monitor(string httpMethod, PathString path);
  }
}
