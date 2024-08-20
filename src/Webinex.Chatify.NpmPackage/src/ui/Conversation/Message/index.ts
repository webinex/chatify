import { MessageAuthor } from './MessageAuthor';
import { MessageBox } from './MessageBox';
import { MessageContent } from './MessageContent';
import { MessageFile } from './MessageFile';
import { MessageFileList } from './MessageFileList';
import { MessageInfoBox } from './MessageInfoBox';
import { MessageRow } from './MessageRow';
import { MessageText } from './MessageText';

export * from './MessageAuthor';
export * from './MessageBox';
export * from './MessageContent';
export * from './MessageInfoBox';
export * from './MessageRow';
export * from './MessageText';
export * from './MessageFileList';
export * from './MessageFile';

export interface MessageCustomizeValue {
  MessageAuthor?: typeof MessageAuthor.Component | null;
  MessageBox?: typeof MessageBox.Component | null;
  MessageContent?: typeof MessageContent.Component | null;
  MessageInfoBox?: typeof MessageInfoBox.Component | null;
  MessageRow?: typeof MessageRow.Component | null;
  MessageText?: typeof MessageText.Component | null;
  MessageFileList?: typeof MessageFileList.Component | null;
  MessageFile?: typeof MessageFile.Component | null;
}
