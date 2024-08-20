import { InputBox } from './InputBox';
import { InputBoxFile } from './InputBoxFile';
import { InputBoxFileList } from './InputBoxFileList';
import { InputFileButton, InputFilesBox } from './InputFilesBox';
import { InputSubmitButtonBox } from './InputSubmitButtonBox';
import { InputTextBox } from './InputTextBox';

export * from './InputBox';
export * from './InputFilesBox';
export * from './InputSubmitButtonBox';
export * from './InputTextBox';
export * from './InputBoxFileList';
export * from './InputBoxFile';

export interface InputBoxCustomizeValue {
  InputBox?: typeof InputBox.Component | null;
  InputTextBox?: typeof InputTextBox.Component | null;
  InputFilesBox?: typeof InputFilesBox.Component | null;
  InputSubmitButtonBox?: typeof InputSubmitButtonBox.Component | null;
  InputBoxFileList?: typeof InputBoxFileList.Component | null;
  InputBoxFile?: typeof InputBoxFile.Component | null;
  InputFileButton?: typeof InputFileButton.Component | null;
}
