import { Button } from 'antd';
import { customize } from '../customize';
import { Icon } from '../Icon';

export interface InputSubmitButtonBoxProps {
  onSubmit: () => void;
}

export const InputSubmitButtonBox = customize('InputSubmitButtonBox', (props: InputSubmitButtonBoxProps) => {
  const { onSubmit } = props;

  return (
    <div className="wxchtf-submit-btn-box">
      <Button onClick={onSubmit} type="link" icon={<Icon type="send" />} className="wxchtf-submit-btn" />
    </div>
  );
});
