import { SendingMessageBox } from './SendingMessageBox';
import { SendingMessageContent } from './SendingMessageContent';
import { SendingMessageInfoBox } from './SendingMessageInfoBox';
import { SendingMessageText } from './SendingMessageText';

export * from './SendingMessageBox';
export * from './SendingMessageContent';
export * from './SendingMessageInfoBox';
export * from './SendingMessageText';

export interface SendingMessageCustomizeValue {
  SendingMessageBox?: typeof SendingMessageBox.Component | null;
  SendingMessageContent?: typeof SendingMessageContent.Component | null;
  SendingMessageInfoBox?: typeof SendingMessageInfoBox.Component | null;
  SendingMessageText?: typeof SendingMessageText.Component | null;
}
