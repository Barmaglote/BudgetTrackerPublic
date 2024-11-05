using budget.core.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using budget.core.Factories.Interfaces;


namespace budget.core.Models {
  public class UserSettingsView : ITransformable<UserSettings> {
    public ObjectId Id { get; set; }
    public string? Language { get; set; }
    public string? Email { get; set; }
    public string? Locale { get; set; }
    public List<UserSubscribtion>? Subscribtions { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdString {
      get { return Convert.ToString(Id); }
      set { Id = ObjectId.Parse(value); }
    }

    public Dictionary<string, string[]>? Categories { get; set; }
    
    public Dictionary<string, TemplateItem[]>? Templates { get; set; }
    public List<AccountItem>? Accounts { get; set; }

    public UserSettingsView(UserSettings item) {
      (Categories, Templates, Accounts, Id, Language, Email, Locale, Subscribtions) = item;
    }

    internal UserSettingsView() { }

    [JsonConstructor]
    public UserSettingsView(
      Dictionary<string, string[]>? categories, 
      Dictionary<string, TemplateItem[]>? templates, 
      List<AccountItem>? accounts,
      string language,
      string email,
      string locale,
      List<UserSubscribtion>? subscribtions
    ) {
      Categories = categories;
      Templates = templates;
      Accounts = accounts;
      Language = language;
      Email = email;
      Locale = locale;
      Subscribtions = subscribtions;
    }

    public UserSettings Transform() => new(this);

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
