using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Models;
using budget.core.Factories.Interfaces;

namespace budget.core.Entities {

  public class UserSettings : BaseItem, ITransformable<UserSettingsView> {

    [BsonElement("categories")]
    public Dictionary<string, string[]>? Categories { get; set; } = new Dictionary<string, string[]>();

    [BsonElement("language")]
    public string? Language { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("locale")]
    public string? Locale { get; set; }

    [BsonElement("subscribtions")]
    public List<UserSubscribtion>? Subscribtions { get; set; } = new List<UserSubscribtion>();

    [BsonElement("templates")]    

    // AREA -> Templates
    public Dictionary<string, TemplateItem[]>? Templates { get; set; } = new Dictionary<string, TemplateItem[]>();
    public List<AccountItem>? Accounts { get; set; }

    public UserSettings() {
    }

    public UserSettings(UserSettingsView view) {
      (Categories, Templates, Accounts, Id, Language, Email, Locale, Subscribtions) = view;
    }

    public UserSettingsView Transform() => new(this);

    public void Deconstruct(
      out Dictionary<string, string[]>? categories,
      out Dictionary<string, TemplateItem[]>? templates,
      out List<AccountItem>? accounts,
      out ObjectId id,
      out string? language,
      out string? email,
      out string? locale,
      out List<UserSubscribtion>? subscriptions
    ) {
      categories = Categories;
      templates = Templates;
      accounts = Accounts;
      id = Id;
      language = Language;
      email = Email;
      locale = Locale;
      subscriptions = Subscribtions;
    }
  }
}
