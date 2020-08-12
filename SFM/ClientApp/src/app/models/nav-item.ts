export class NavGroup {
  public name: string;
  public items: NavItem[];
}

export class NavItem {
  public name: string;
  public action: () => void;
  public icon?: string;
  public loggedIn?: boolean;
}