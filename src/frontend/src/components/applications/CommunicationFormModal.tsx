import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateCommunication, useUpdateCommunication } from '@/api/communications';
import type {
  ApplicationCommunicationDto,
  CreateApplicationCommunicationDto,
  UpdateApplicationCommunicationDto,
} from '@/types';
import { Calendar, FileText, Loader2 } from 'lucide-react';

interface CommunicationFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  applicationId: string;
  communicationToEdit?: ApplicationCommunicationDto | null;
  onSuccess?: (action: 'created' | 'updated') => void;
}

const toLocalDatetimeInputValue = (isoStr?: string | null): string => {
  const date = isoStr ? new Date(isoStr) : new Date();
  if (isNaN(date.getTime())) return '';
  const pad = (n: number) => n.toString().padStart(2, '0');
  const year = date.getFullYear();
  const month = pad(date.getMonth() + 1);
  const day = date.getDate();
  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  return `${year}-${month}-${day}T${hours}:${minutes}`;
};

export const CommunicationFormModal: React.FC<CommunicationFormModalProps> = ({
  isOpen,
  onClose,
  applicationId,
  communicationToEdit,
  onSuccess,
}) => {
  const isEditing = Boolean(communicationToEdit);

  const createMutation = useCreateCommunication();
  const updateMutation = useUpdateCommunication();
  const activeMutation = isEditing ? updateMutation : createMutation;

  const [occurredAt, setOccurredAt] = useState('');
  const [notes, setNotes] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (communicationToEdit) {
      setOccurredAt(toLocalDatetimeInputValue(communicationToEdit.occurredAt));
      setNotes(communicationToEdit.notes ?? '');
    } else {
      setOccurredAt(toLocalDatetimeInputValue());
      setNotes('');
    }
    setValidationError(null);
    createMutation.reset();
    updateMutation.reset();
  }, [communicationToEdit, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    if (!occurredAt) {
      setValidationError('Occurrence date and time are required.');
      return;
    }

    const trimmedNotes = notes.trim();
    if (!trimmedNotes) {
      setValidationError('Notes are required.');
      return;
    }

    if (trimmedNotes.length > 4000) {
      setValidationError('Notes must not exceed 4000 characters.');
      return;
    }

    // Backend domain requirement: occurredAt must be in UTC
    const occurredAtUtc = new Date(occurredAt).toISOString();

    try {
      if (isEditing && communicationToEdit) {
        const updateDto: UpdateApplicationCommunicationDto = {
          occurredAt: occurredAtUtc,
          notes: trimmedNotes,
        };

        await updateMutation.mutateAsync({
          applicationId,
          communicationId: communicationToEdit.id,
          dto: updateDto,
        });

        onSuccess?.('updated');
      } else {
        const createDto: CreateApplicationCommunicationDto = {
          occurredAt: occurredAtUtc,
          notes: trimmedNotes,
        };

        await createMutation.mutateAsync({
          applicationId,
          dto: createDto,
        });

        onSuccess?.('created');
      }

      onClose();
    } catch {
      // Handled by activeMutation.error
    }
  };

  const remainingChars = 4000 - notes.length;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Communication' : 'Log Communication'}
      description="Record correspondence, emails, recruiter messages, follow-up calls, or updates."
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {validationError && (
          <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
            {validationError}
          </div>
        )}

        {activeMutation.isError && <ErrorAlert error={activeMutation.error} />}

        {/* Occurrence Date & Time */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
            <Calendar className="h-3.5 w-3.5 text-primary" />
            <span>Date & Time Occurred <span className="text-destructive">*</span></span>
          </label>
          <input
            type="datetime-local"
            value={occurredAt}
            onChange={(e) => setOccurredAt(e.target.value)}
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            required
          />
          <p className="text-[11px] text-muted-foreground">
            Automatically converted to UTC upon saving to meet backend audit rules.
          </p>
        </div>

        {/* Notes */}
        <div className="space-y-1.5">
          <div className="flex items-center justify-between">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <FileText className="h-3.5 w-3.5 text-primary" />
              <span>Communication Notes <span className="text-destructive">*</span></span>
            </label>
            <span
              className={`text-[11px] ${
                remainingChars < 0
                  ? 'text-destructive font-bold'
                  : remainingChars < 200
                  ? 'text-amber-400'
                  : 'text-muted-foreground'
              }`}
            >
              {notes.length} / 4000 characters
            </span>
          </div>
          <textarea
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            rows={5}
            maxLength={4100}
            placeholder="Log email conversation, LinkedIn recruiter response, salary discussion, technical assignment submission, or feedback..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary leading-relaxed"
            required
          />
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-2 pt-3 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            disabled={activeMutation.isPending}
            className="rounded-lg px-3 py-1.5 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={activeMutation.isPending}
            className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50"
          >
            {activeMutation.isPending && <Loader2 className="h-3.5 w-3.5 animate-spin" />}
            {isEditing ? 'Save Changes' : 'Log Communication'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default CommunicationFormModal;
