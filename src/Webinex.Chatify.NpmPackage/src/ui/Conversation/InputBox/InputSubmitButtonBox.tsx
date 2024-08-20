import { Button } from 'antd';
import { customize } from '../../customize';
import { Icon } from '../../common/Icon';

export interface InputSubmitButtonBoxProps {
  onSubmit: () => void;
  disabled?: boolean;
}

export const InputSubmitButtonBox = customize('InputSubmitButtonBox', (props: InputSubmitButtonBoxProps) => {
  const { onSubmit, disabled } = props;

  return (
    <div className="wxchtf-submit-btn-box">
      <Button
        onClick={onSubmit}
        type="link"
        icon={<Icon type="send" />}
        className="wxchtf-submit-btn"
        disabled={disabled}
      />
    </div>
  );
});
