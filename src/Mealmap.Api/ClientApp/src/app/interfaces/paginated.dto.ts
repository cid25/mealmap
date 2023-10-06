export interface Paginated<Type> {
  items: Type[];
  next: URL;
  previous: URL;
}
