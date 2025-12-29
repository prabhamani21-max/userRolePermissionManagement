
export type MenuItem = {
  key?: string;
  label?: string;
  icon?: string;
  link?: string;
  collapsed?: boolean;
  subMenu?: MenuItem[];
  isTitle?: boolean;
  badge?: any;
  queryParams?: { [key: string]: any };
  parentKey?: string;
  disabled?: boolean;
  roleId?: number[]; // Changed from roles to roleId with number[]
};

