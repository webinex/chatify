import { Button, Tooltip } from 'antd';
import { customize } from '../customize';
import { useThreadContext } from './ThreadContext';
import { Icon } from '../common/Icon';

export const ThreadActions = customize('ThreadActions', () => {
  const { watch, onWatch } = useThreadContext();

  if (!watch) {
    return (
      <Tooltip title="Watch thread">
        <Button type="link" icon={<Icon type="watch" />} onClick={() => onWatch(true)} />
      </Tooltip>
    );
  }

  return (
    <Tooltip title="Unwatch thread">
      <Button type="link" icon={<Icon type="unwatch" />} onClick={() => onWatch(false)} />
    </Tooltip>
  );
});
