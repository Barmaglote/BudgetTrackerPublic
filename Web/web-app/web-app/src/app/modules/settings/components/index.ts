import { AccountsEditorComponent } from "./accounts-editor/accounts-editor.component";
import { CategoriesListComponent } from "./categories-list/categories-list.component";
import { LanguageSelectorComponent } from "./language-selector/language-selector.component";
import { LocaleSelectorComponent } from "./locale-selector/locale-selector.component";
import { TemplatesEditorComponent } from "./templates-editor/templates-editor.component";

export const SETTINGS_COMPONENTS = [
  AccountsEditorComponent,
  CategoriesListComponent,
  LanguageSelectorComponent,
  LocaleSelectorComponent,
  TemplatesEditorComponent
];

export * from "./accounts-editor/accounts-editor.component";
export * from "./categories-list/categories-list.component";
export * from "./templates-editor/templates-editor.component";
export * from "./language-selector/language-selector.component";
export * from "./locale-selector/locale-selector.component";
