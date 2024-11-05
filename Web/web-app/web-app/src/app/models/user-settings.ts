import ObjectID from "bson-objectid";
import { TemplateItem } from "../shared/models/template-item";
import { AccountItem } from "../shared/models/account-item";
import {  UserSubscribtion } from "./user-subscribtion"

export interface UserSettings {
  id: ObjectID;
  idString: string;
  userId: string;
  email: string;
  categories?: Record<string, string[]> | null;
  templates?: Record<string, TemplateItem[]> | null;
  accounts?: AccountItem[] | [];
  language: string;
  locale: string;
  subscribtions: UserSubscribtion[] | null;
}
