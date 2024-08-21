import { Button } from 'antd';
import { File } from '../../../core';
import { Icon } from '../../common';
import { useCallback } from 'react';
import { useFormatter } from '../../useFormatter';
import { customize } from '../../customize';

export interface InputBoxFileProps {
  file: File;
  disabled?: boolean;
  onDelete: (file: File) => void;
}

export const InputBoxFile = customize('InputBoxFile', (props: InputBoxFileProps) => {
  const { file, disabled, onDelete } = props;
  const formatter = useFormatter();
  const onDeleteClick = useCallback(() => onDelete(file), [file, onDelete]);

  return (
    <div className="wxchtf-file">
      <div className="wxchtf-file-info">
        <div className="wxchtf-file-name" title={file.name}>
          {file.name}
        </div>
        <div className="wxchtf-file-size">{formatter.size(file.bytes)}</div>
      </div>
      <div className="wxchtf-file-actions">
        <Button
          disabled={disabled}
          className="wxchtf-delete"
          type="link"
          icon={<Icon type="delete-file" />}
          onClick={onDeleteClick}
        />
      </div>
    </div>
  );
});
