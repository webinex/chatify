import classNames from 'classnames';
import { AuditSearch } from './AuditSearch';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { AuditSearchUtil } from './AuditSearchValue';
import { auditApi } from './auditApi';
import { AuditChatList } from './AuditChatList';
import { CustomizeContext, defaultLocalizer, Localizer, LocalizerContext } from '@webinex/chatify';
import { AuditConversationPanel } from './AuditConversationPanel';
import { AuditPanelCustomizeValue } from './AuditPanelCustomizeValue';

export interface AuditPanelProps {
  className?: string;
  style?: React.CSSProperties;
  localizer?: Localizer;
  customize?: AuditPanelCustomizeValue;

  /**
   * Maximum number of chats to load.
   * @default 100
   */
  max?: number;
}

function useData(props: AuditPanelProps, values: string[]) {
  const { max } = props;
  const query = useMemo(() => AuditSearchUtil.queryOf(values), [values]);

  const { currentData: chats, isLoading } = auditApi.useGetAuditChatListQuery(
    {
      ...query!,
      pagingRule: { skip: 0, take: max! },
      includeTotal: true,
    },
    { skip: !query },
  );

  return useMemo(() => ({ chats: chats?.items ?? [], isLoading }), [chats, isLoading]);
}

const EMPTY = Object.freeze({} as AuditPanelCustomizeValue);

export function AuditPanel(props: AuditPanelProps) {
  props = { max: 100, customize: EMPTY, ...props };
  const { className, style, localizer = defaultLocalizer, customize = EMPTY, max } = props;
  const [values, setValues] = useState<string[]>([]);
  const { chats, isLoading } = useData(props, values);

  const [selectedId, setSelectedId] = useState<string | null>(null);
  const resetSelected = useCallback(() => setSelectedId(null), []);

  useEffect(() => {
    if (chats.every((x) => x.id !== selectedId)) {
      setSelectedId(null);
    }
  }, [chats, selectedId]);

  return (
    <CustomizeContext.Provider value={customize}>
      <LocalizerContext.Provider value={localizer}>
        <div className={classNames(className, 'wxchtf-audit-panel')} style={style}>
          <AuditSearch onSearch={setValues} />
          <div className={classNames('wxchtf-audit-body', { '--conversation-opened': !!selectedId })}>
            <AuditChatList
              items={chats}
              loading={isLoading}
              selected={selectedId}
              onSelect={setSelectedId}
              top={Math.min(chats.length, max!)}
            />
            {selectedId && <AuditConversationPanel id={selectedId} onClose={resetSelected} />}
          </div>
        </div>
      </LocalizerContext.Provider>
    </CustomizeContext.Provider>
  );
}
