import { AccountsComponent } from "./accounts/accounts.component";
import { CategoriesComponent } from "./categories/categories.component";
import { LanguageComponent } from "./language/language.component";
import { SettingRootComponent } from "./settings-root/settings-root.component";
import { SettingsComponent } from "./settings/settings.component";
import { TemplatesComponent } from "./templates/templates.component";

export const SETTINGS_CONTAINERS = [
  AccountsComponent,
  CategoriesComponent,
  SettingsComponent,
  SettingRootComponent,
  TemplatesComponent,
  LanguageComponent
]

export * from "./accounts/accounts.component";
export * from "./categories/categories.component";
export * from "./settings-root/settings-root.component";
export * from "./settings/settings.component";
export * from "./templates/templates.component";
export * from "./language/language.component";
