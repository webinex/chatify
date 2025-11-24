import { Account, chatifyApi, customize, Localizer, useLocalizer } from '@webinex/chatify';
import { Select } from 'antd';
import classNames from 'classnames';
import { useEffect, useMemo, useState } from 'react';
import { AuditSearchUtil } from './AuditSearchValue';

export interface AuditSearchProps {
  className?: string;
  onSearch?: (values: string[]) => void;
}

type Value = { label: JSX.Element; value: string };

function* mapOptions(localizer: Localizer, accounts: Account[] | undefined, searchValue: string) {
  if (searchValue.length === 0) return;

  if (searchValue.length > 0)
    yield {
      label: localizer.audit.textSearchOption(searchValue),
      value: AuditSearchUtil.TEXT_PREFIX + searchValue,
    };

  for (const account of accounts?.filter((x) =>
    x.name.toLocaleLowerCase().includes(searchValue.toLocaleLowerCase()),
  ) ?? []) {
    yield {
      label: localizer.audit.memberSearchOption(account),
      value: AuditSearchUtil.MEMBER_PREFIX + account.id,
    };
  }
}

function useOptions(accounts: Account[] | undefined, searchValue: string) {
  const localizer = useLocalizer();
  return useMemo(
    () => Array.from(mapOptions(localizer, accounts, searchValue)),
    [localizer, accounts, searchValue],
  );
}

export const AuditSearch = customize('AuditSearch', (props: AuditSearchProps) => {
  const { className, onSearch } = props;
  const [searchValue, setSearchValue] = useState('');
  const [values, setValues] = useState<Value[]>([]);
  const { data: accounts, isLoading } = chatifyApi.useGetAccountListQuery();
  const options = useOptions(accounts, searchValue);
  const localizer = useLocalizer();

  useEffect(() => {
    onSearch?.(values.map((x) => x.value));
  }, [onSearch, values]);

  return (
    <div className={classNames('wxchtf-search', className)}>
      <Select<string[], Value>
        allowClear
        className="wxchtf-search-select"
        mode="multiple"
        value={values.map((x) => x.value)}
        options={options.length > 0 ? options : values}
        searchValue={searchValue}
        onSearch={setSearchValue}
        onSelect={(_, option) => {
          setValues((prev) => {
            if (AuditSearchUtil.isSearchByText(option.value)) {
              prev = prev.filter((x) => !AuditSearchUtil.isSearchByText(x.value));
            }

            return [...prev, option];
          });
          setSearchValue('');
        }}
        onDeselect={(_, option) => {
          setValues((prev) => prev.filter((x) => x.value !== option.value));
          setSearchValue('');
        }}
        filterOption={false}
        onDropdownVisibleChange={() => setSearchValue('')}
        disabled={isLoading}
        loading={isLoading}
        placeholder={localizer.audit.searchPlaceholder}
        notFoundContent={
          searchValue.length === 0 ? localizer.audit.noInputBlurb : localizer.audit.noChatFound
        }
        size="large"
      />
    </div>
  );
});
