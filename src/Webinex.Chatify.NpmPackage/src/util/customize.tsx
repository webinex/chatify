import React, { FC, PropsWithChildren, useContext } from 'react';

export type Customizable<TComponent extends React.ComponentType<any>, TKey extends string> = TComponent & {
  Component: TComponent;
  key: TKey;
};

const CustomizeContext = React.createContext<CustomizeProviderValue>(null!);

export interface CustomizeProviderValue {
  [key: string]: React.ComponentType<any> | null;
}

export function CustomizeProvider(props: PropsWithChildren<{ value: CustomizeProviderValue }>) {
  const { children, value } = props;
  return <CustomizeContext.Provider value={value}>{children}</CustomizeContext.Provider>;
}

function useCustom<TComponent extends React.ComponentType>(key: string): TComponent | undefined | null {
  const value = useContext(CustomizeContext);
  return value[key] as TComponent | null | undefined;
}

export function customize<TComponent extends React.ComponentType<any>, TKey extends string>(
  key: TKey,
  Component: TComponent,
): Customizable<TComponent, TKey> {
  const Result: FC<any> = React.forwardRef<TComponent, any>((props: any, forwardRef) => {
    const Custom = useCustom(key);

    if (Custom === null) {
      return null;
    }

    if (Custom) {
      return <Custom {...props} ref={forwardRef} />;
    }

    return <Component {...props} ref={forwardRef} />;
  });

  Result.displayName = 'Customizable.' + Component.displayName;

  return Object.assign(Result, { key, Component }) as any;
}
