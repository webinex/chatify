import { GetAuditChatListQueryArgs } from './auditApi';

export interface AuditSearchOption {
  label: JSX.Element;
  value: string;
}

const MEMBER_PREFIX = 'member://';
const TEXT_PREFIX = 'text://';

function queryOf(
  values: string[],
): Pick<GetAuditChatListQueryArgs, 'containsOneOfMembers' | 'searchString'> | null {
  let containsOneOfMembers: string[] | null = values
    .filter(AuditSearchUtil.isSearchByMember)
    .map((v) => v.slice(MEMBER_PREFIX.length));

  containsOneOfMembers = containsOneOfMembers.length > 0 ? containsOneOfMembers : null;

  const searchStringValues = values
    .filter(AuditSearchUtil.isSearchByText)
    .map((v) => v.slice(TEXT_PREFIX.length));

  const searchString = searchStringValues.length > 0 ? searchStringValues.join(' ') : null;

  if (!containsOneOfMembers && !searchString) {
    return null;
  }

  return {
    containsOneOfMembers,
    searchString,
  };
}

export const AuditSearchUtil = Object.freeze({
  MEMBER_PREFIX,
  TEXT_PREFIX,
  isSearchByMember: (value: string) => value.startsWith(MEMBER_PREFIX),
  isSearchByText: (value: string) => value.startsWith(TEXT_PREFIX),
  queryOf,
});
