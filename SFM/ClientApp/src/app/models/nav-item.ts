export class NavGroup {
  name: string;
  items: NavItem[];
}

export class NavItem {
  name: string;
  action: () => void;
  icon?: string;
}