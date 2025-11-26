import { AutoReplyButton } from './AutoReplyButton';
import { AutoReplyForm } from './AutoReplyForm';
import { AutoReplyPanel } from './AutoReplyPanel';
import { AutoReplyPeriodInput } from './AutoReplyPeriodInput';
import { AutoReplyTextInput } from './AutoReplyTextInput';

export * from './AutoReplyButton';
export * from './AutoReplyForm';
export * from './AutoReplyPanel';
export * from './AutoReplyPeriodInput';
export * from './AutoReplyTextInput';

export interface AutoReplyCustomizeValue {
  AutoReplyButton?: typeof AutoReplyButton.Component | null;
  AutoReplyForm?: typeof AutoReplyForm.Component | null;
  AutoReplyPanel?: typeof AutoReplyPanel.Component | null;
  AutoReplyPeriodInput?: typeof AutoReplyPeriodInput.Component | null;
  AutoReplyTextInput?: typeof AutoReplyTextInput.Component | null;
}
