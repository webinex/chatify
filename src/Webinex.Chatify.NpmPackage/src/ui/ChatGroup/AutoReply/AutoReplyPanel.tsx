import dayjs from 'dayjs';
import { useCallback, useMemo } from 'react';
import { Button } from 'antd';
import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';
import { useChatGroupContext } from '../ChatGroupContext';
import { Icon } from '../../common/Icon';
import { AutoReply } from '../../../core';
import { chatifyApi } from '../../../core';
import { AUTO_REPLY_FORM_INITIAL_VALUE, AutoReplyForm, AutoReplyFormValue } from './AutoReplyForm';

function mapToFormValue(autoReply?: AutoReply | null): AutoReplyFormValue {
  if (!autoReply) {
    return AUTO_REPLY_FORM_INITIAL_VALUE;
  }

  return {
    period: [dayjs(autoReply.period.start), dayjs(autoReply.period.end)],
    text: autoReply.text,
  };
}

function useValue() {
  const meId = chatifyApi.settings.me();
  const { data: accounts } = chatifyApi.useGetAccountListQuery();

  const value = useMemo<AutoReplyFormValue>(() => {
    const me = accounts?.find((x) => x.id === meId);

    return mapToFormValue(me?.autoReply);
  }, [accounts, meId]);

  return value;
}

function useAutoReplyActions() {
  const meId = chatifyApi.settings.me();
  const [updateAccount, { isLoading }] = chatifyApi.useUpdateAccountMutation();

  const onSubmit = useCallback(
    (value: AutoReplyFormValue) =>
      updateAccount({
        id: meId,
        autoReply: {
          hasValue: true,
          value: {
            text: value.text,
            period: {
              start: value.period[0].toISOString(),
              end: value.period[1].toISOString(),
            },
          },
        },
      }).unwrap(),
    [updateAccount, meId],
  );

  const onClear = useCallback(
    () =>
      updateAccount({
        id: meId,
        autoReply: {
          hasValue: true,
          value: null,
        },
      }).unwrap(),
    [updateAccount, meId],
  );

  return [onSubmit, isLoading, onClear] as const;
}

export const AutoReplyPanel = customize('AutoReplyPanel', () => {
  const localizer = useLocalizer();
  const { toggleAutoReply } = useChatGroupContext();
  const value = useValue();
  const [onSubmit, submitting, onClear] = useAutoReplyActions();

  return (
    <div className="wxchtf-auto-reply-panel">
      <Button
        onClick={() => toggleAutoReply()}
        className="wxchtf-auto-reply-panel-close"
        type="link"
        icon={<Icon type="close" />}
      />
      <div className="wxchtf-auto-reply-panel-header">
        <h3>{localizer.autoReply.title()}</h3>
        <Button type="link" onClick={onClear} disabled={submitting} className="wxchtf-auto-reply-panel-clear">
          {localizer.autoReply.clearBtn()}
        </Button>
      </div>
      <AutoReplyForm value={value} busy={submitting ?? false} onSubmit={onSubmit} />
    </div>
  );
});
