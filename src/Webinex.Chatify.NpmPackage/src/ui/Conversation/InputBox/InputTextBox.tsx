import { Input } from 'antd';
import { customize } from '../../customize';
import isHotkey from 'is-hotkey';
import { RefObject, memo, useCallback } from 'react';
import { useLocalizer } from '../../localizer';
import { TextAreaProps } from 'antd/es/input';

export interface TextInputBoxProps {
  value: string;
  onChange: (value: string) => void;
  onSend: (value: string) => void;
  disabled?: boolean;
  inputRef?: RefObject<HTMLTextAreaElement>;
}

const isSend = isHotkey('ctrl+enter');

const AUTO_SIZE: TextAreaProps['autoSize'] = { maxRows: 5, minRows: 1 };

export const InputTextBox = customize(
  'InputTextBox',
  memo((props: TextInputBoxProps) => {
    const { value, onChange, onSend, disabled, inputRef } = props;
    const localizer = useLocalizer();

    const onInputChange = useCallback(
      (e: React.ChangeEvent<HTMLTextAreaElement>) => onChange(e.target.value),
      [onChange],
    );

    const onKeyDown = useCallback(
      (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (isSend(e)) {
          onSend(value);
        }
      },
      [onSend, value],
    );

    return (
      <div className="wxchtf-text-input-box">
        <Input.TextArea
          ref={inputRef}
          value={value}
          onChange={onInputChange}
          autoSize={AUTO_SIZE}
          className="wxchtf-text-input-textarea"
          placeholder={localizer.input.placeholder()}
          onKeyDown={onKeyDown}
          disabled={disabled}
        />
      </div>
    );
  }),
);
