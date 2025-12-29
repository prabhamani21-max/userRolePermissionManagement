import { MenuItem } from './menu-meta';

/**
 * Finds all parent menu items for a given menu item
 * @param menuItems The full menu array
 * @param menuItem The menu item to find parents for
 * @returns Array of parent keys
 */
export function findAllParent(menuItems: MenuItem[], menuItem: MenuItem): string[] {
  const parents: string[] = [];
  let currentItem = menuItem;

  while (currentItem.parentKey) {
    const parent = findMenuItem(menuItems, currentItem.parentKey);
    if (parent) {
      parents.unshift(parent.key || '');
      currentItem = parent;
    } else {
      break;
    }
  }

  return parents;
}

/**
 * Finds a menu item by its key
 * @param menuItems The menu array to search
 * @param key The key to find
 * @returns The menu item or null
 */
export function findMenuItem(menuItems: MenuItem[], key: string): MenuItem | null {
  for (const item of menuItems) {
    if (String(item.key) === key) {
      return item;
    }
    if (item.subMenu) {
      const found = findMenuItem(item.subMenu, key);
      if (found) return found;
    }
  }
  return null;
}