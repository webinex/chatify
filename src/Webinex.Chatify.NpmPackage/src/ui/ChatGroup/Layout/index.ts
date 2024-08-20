import { ChatGroupAside } from './ChatGroupAside';
import { ChatGroupHeader } from './ChatGroupHeader';
import { ChatGroupFooter } from './ChatGroupFooter';
import { ChatGroupMain } from './ChatGroupMain';
import { ChatGroupBody } from './ChatGroupBody';

export * from './ChatGroupAside';
export * from './ChatGroupHeader';
export * from './ChatGroupFooter';
export * from './ChatGroupMain';
export * from './ChatGroupBody';

export interface ChatGroupLayoutCustomizeValue {
  ChatGroupAside?: typeof ChatGroupAside.Component | null;
  ChatGroupHeader?: typeof ChatGroupHeader.Component | null;
  ChatGroupFooter?: typeof ChatGroupFooter.Component | null;
  ChatGroupMain?: typeof ChatGroupMain.Component | null;
  ChatGroupBody?: typeof ChatGroupBody.Component | null;
}
