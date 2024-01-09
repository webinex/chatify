import { useClient } from '../ChatifyContext';
import { AddMemberRequest } from './models';
import { useMutation } from './useMutation';

export function useAddMember() {
  const client = useClient();

  return useMutation({
    key: 'add-member',
    queryFn: (args: AddMemberRequest) => client.addMember(args),
  });
}
