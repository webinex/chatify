import React from 'react';
import { Avatar, Select } from 'antd';
import { Account } from '../../src';

export interface LoginPageProps {
  onLoggedIn: (id: string) => void;
}

export function LoginPage(props: LoginPageProps) {
  const { onLoggedIn } = props;

  const [accounts, setAccounts] = React.useState<Account[]>([]);

  React.useEffect(() => {
    fetch('/api/account')
      .then((x) => x.json())
      .then((x) => setAccounts(x));
  }, []);

  return (
    <div className="example">
      <div className="user-select-box">
        <h3>Please select user</h3>
        <Select
          className="user-select"
          placeholder="Select user..."
          onChange={onLoggedIn}
          options={accounts.map((x) => ({
            value: x.id,
            label: (
              <span>
                <Avatar src={x.avatar} /> {x.name}
              </span>
            ),
          }))}
        />
      </div>
    </div>
  );
}
