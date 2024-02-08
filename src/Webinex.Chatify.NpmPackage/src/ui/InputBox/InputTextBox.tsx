import { Input } from 'antd';
import { customize } from '../customize';
import isHotkey from 'is-hotkey';
import { memo, useCallback } from 'react';
import { useLocalizer } from '../localizer';

export interface TextInputBoxProps {
  value: string;
  onChange: (value: string) => void;
  onSend: (value: string) => void;
}

const isSend = isHotkey('ctrl+enter');

const AUTO_SIZE = { maxRows: 5 };

export const InputTextBox = customize(
  'InputTextBox',
  memo((props: TextInputBoxProps) => {
    const { value, onChange, onSend } = props;
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
          value={value}
          onChange={onInputChange}
          autoSize={AUTO_SIZE}
          className="wxchtf-text-input-textarea"
          placeholder={localizer.input.placeholder()}
          onKeyDown={onKeyDown}
        />
      </div>
    );
  }),
);
