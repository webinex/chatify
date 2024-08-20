import { SystemMessageBox } from './SystemMessageBox';
import { SystemMessageRow } from './SystemMessageRow';

export * from './SystemMessageBox';
export * from './SystemMessageRow';

export interface SystemMessageCustomizeValue {
  SystemMessageBox?: typeof SystemMessageBox.Component | null;
  SystemMessageRow?: typeof SystemMessageRow.Component | null;
}
