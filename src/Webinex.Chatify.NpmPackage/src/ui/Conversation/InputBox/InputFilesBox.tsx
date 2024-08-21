import { Button } from 'antd';
import { customize } from '../../customize';
import { Icon } from '../../common/Icon';
import { File } from '../../../core';
import { useCallback, useEffect, useRef } from 'react';

export interface InputFilesBoxProps {
  value: File[];
  accept?: string;
  loading?: boolean;
  disabled?: boolean;
  onAddedClick?: () => void;
  onUpload: (value: globalThis.File[]) => void;
}

export const InputFileButton = customize('InputFileButton', (props: InputFilesBoxProps) => {
  const { onUpload, onAddedClick, accept, loading, value, disabled } = props;
  const inputRef = useRef<HTMLInputElement | null>(null);
  const onClick = useCallback(() => inputRef.current?.click(), []);
  const countButtonRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    countButtonRef.current?.classList.add('active');
  }, [value]);

  const onCountButtonAnimationEnd = useCallback(() => {
    countButtonRef.current?.classList.remove('active');
  }, []);

  const onInputChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files;
      files && files.length > 0 && onUpload(Array.from(files));
    },
    [onUpload],
  );

  return (
    <>
      <Button
        loading={loading}
        onClick={onClick}
        type="link"
        disabled={disabled}
        icon={<Icon type="attach" />}
        className="wxchtf-add-files-btn"
      />
      {value.length > 0 && (
        <Button
          ref={countButtonRef}
          className="wxchtf-add-files-added-btn"
          onAnimationIteration={onCountButtonAnimationEnd}
          onClick={onAddedClick}
          type="link"
          disabled={disabled}
          icon={<>+{value.length}</>}
        />
      )}
      <input
        value=""
        accept={accept}
        onChange={onInputChange}
        ref={inputRef}
        type="file"
        multiple
        style={{ display: 'none' }}
      />
    </>
  );
});

export const InputFilesBox = customize('InputFilesBox', (props: InputFilesBoxProps) => {
  return (
    <div className="wxchtf-add-files-box">
      <InputFileButton {...props} />
    </div>
  );
});
