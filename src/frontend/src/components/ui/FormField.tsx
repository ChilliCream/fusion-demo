import type { ReactNode } from "react";

interface FormFieldProps {
  /** `id` of the control this label points at. */
  htmlFor: string;
  label?: ReactNode;
  required?: boolean;
  error?: string;
  children: ReactNode;
}

/**
 * Wraps a form control with an optional label (with a required marker) and
 * an error message. Ported from `FormField.tsx` in the website design
 * system; used by `Input` to keep its chrome in sync.
 */
export function FormField({
  htmlFor,
  label,
  required,
  error,
  children,
}: FormFieldProps) {
  return (
    <div className="flex flex-col gap-1">
      {label && (
        <label htmlFor={htmlFor} className="text-cc-ink text-sm font-medium">
          {label}
          {required && <span className="text-cc-accent ml-1">*</span>}
        </label>
      )}
      {children}
      {error && <span className="text-sm text-cc-danger">{error}</span>}
    </div>
  );
}
