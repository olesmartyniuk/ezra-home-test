import { useTaskContext } from '../../context/TaskContext';
import { Select } from '../ui/Select';
import { Input } from '../ui/Input';
import { Button } from '../ui/Button';

const statusOptions = [
  { value: '', label: 'All Statuses' },
  { value: 'Todo', label: 'To Do' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'Done', label: 'Done' },
];

const priorityOptions = [
  { value: '', label: 'All Priorities' },
  { value: 'Low', label: 'Low' },
  { value: 'Medium', label: 'Medium' },
  { value: 'High', label: 'High' },
];

const sortByOptions = [
  { value: 'createdAt', label: 'Date Created' },
  { value: 'dueDate', label: 'Due Date' },
  { value: 'priority', label: 'Priority' },
  { value: 'status', label: 'Status' },
];

const sortOrderOptions = [
  { value: 'desc', label: 'Descending' },
  { value: 'asc', label: 'Ascending' },
];

export function TaskFilters() {
  const { state, setFilters } = useTaskContext();
  const { filters } = state;

  function handleReset() {
    setFilters({ status: '', priority: '', search: '', sortBy: 'createdAt', sortOrder: 'desc' });
  }

  const hasActiveFilters = filters.status || filters.priority || filters.search;

  return (
    <div className="bg-white rounded-xl border shadow-sm p-4 flex flex-col sm:flex-row gap-3">
      <div className="flex-1 grid grid-cols-1 sm:grid-cols-3 gap-3">
        <Input
          id="search"
          placeholder="Search tasks..."
          value={filters.search ?? ''}
          onChange={(e) => setFilters({ search: e.target.value })}
        />
        <Select
          id="status-filter"
          options={statusOptions}
          value={filters.status ?? ''}
          onChange={(e) => setFilters({ status: e.target.value as typeof filters.status })}
        />
        <Select
          id="priority-filter"
          options={priorityOptions}
          value={filters.priority ?? ''}
          onChange={(e) => setFilters({ priority: e.target.value as typeof filters.priority })}
        />
      </div>

      <div className="border-t sm:border-t-0 sm:border-l border-gray-200 pt-3 sm:pt-0 sm:pl-3 flex items-end gap-2 shrink-0">
        <Select
          id="sort-by"
          label="Sort by"
          options={sortByOptions}
          value={filters.sortBy ?? 'createdAt'}
          onChange={(e) => setFilters({ sortBy: e.target.value as typeof filters.sortBy })}
          className="w-36"
        />
        <Select
          id="sort-order"
          label="Direction"
          options={sortOrderOptions}
          value={filters.sortOrder ?? 'desc'}
          onChange={(e) => setFilters({ sortOrder: e.target.value as typeof filters.sortOrder })}
          className="w-32"
        />
      </div>

      {hasActiveFilters && (
        <div className="border-t sm:border-t-0 sm:border-l border-gray-200 pt-3 sm:pt-0 sm:pl-3 flex items-end shrink-0">
          <Button variant="ghost" size="sm" onClick={handleReset}>
            Clear filters
          </Button>
        </div>
      )}
    </div>
  );
}
