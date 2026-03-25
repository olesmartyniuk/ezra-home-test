import { z } from 'zod';

export const taskSchema = z.object({
  title: z
    .string()
    .min(1, 'Title is required.')
    .max(200, 'Title must not exceed 200 characters.'),
  description: z
    .string()
    .max(2000, 'Description must not exceed 2000 characters.')
    .nullable()
    .optional(),
  status: z.enum(['Todo', 'InProgress', 'Done']),
  priority: z.enum(['Low', 'Medium', 'High']),
  dueDate: z
    .string()
    .nullable()
    .optional()
    .refine(
      (val) => !val || new Date(val) >= new Date(new Date().toDateString()),
      { message: 'Due date must be today or in the future.' }
    ),
});

export type TaskFormValues = z.infer<typeof taskSchema>;
