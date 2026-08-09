import React from 'react';
import { AlertTriangle } from 'lucide-react';
import type { ProblemDetails } from '@/types';
import { isAxiosError } from 'axios';

interface ErrorAlertProps {
  error: unknown;
  onDismiss?: () => void;
}

export const ErrorAlert: React.FC<ErrorAlertProps> = ({ error, onDismiss }) => {
  if (!error) return null;

  let title = 'An error occurred';
  let detail = 'Please try again or check your network connection.';
  let validationErrors: Record<string, string[]> | undefined;

  if (isAxiosError(error) && error.response?.data) {
    const problem = error.response.data as ProblemDetails;
    if (problem.title) title = problem.title;
    if (problem.detail) detail = problem.detail;
    if (problem.errors) validationErrors = problem.errors;
  } else if (error instanceof Error) {
    detail = error.message;
  }

  return (
    <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-4 text-destructive-foreground">
      <div className="flex items-start gap-3">
        <AlertTriangle className="h-5 w-5 text-destructive shrink-0 mt-0.5" />
        <div className="flex-1 text-sm">
          <h4 className="font-semibold text-destructive">{title}</h4>
          <p className="mt-1 text-muted-foreground">{detail}</p>

          {validationErrors && Object.keys(validationErrors).length > 0 && (
            <ul className="mt-2 space-y-1 list-disc list-inside text-xs text-destructive">
              {Object.entries(validationErrors).map(([field, messages]) => (
                <li key={field}>
                  <strong className="capitalize">{field}</strong>: {messages.join(', ')}
                </li>
              ))}
            </ul>
          )}
        </div>
        {onDismiss && (
          <button
            type="button"
            onClick={onDismiss}
            className="text-xs text-muted-foreground hover:text-foreground"
          >
            Dismiss
          </button>
        )}
      </div>
    </div>
  );
};

export default ErrorAlert;
