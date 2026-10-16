import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateInterview, useUpdateInterview } from '@/api/interviews';
import type { InterviewDto, InterviewType, CreateInterviewDto, UpdateInterviewDto } from '@/types';
import { INTERVIEW_TYPE_CONFIG } from './InterviewCard';
import { Calendar, CheckCircle2, FileText, MessageSquare, Loader2 } from 'lucide-react';

interface InterviewFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  applicationId: string;
  interviewToEdit?: InterviewDto | null;
  onSuccess?: (action: 'created' | 'updated') => void;
}

const INTERVIEW_TYPE_OPTIONS: InterviewType[] = [
  'PhoneScreen',
  'HRInterview',
  'TechnicalInterview',
  'SystemDesign',
  'CodingChallenge',
  'FinalRound',
];

// Helper to convert ISO UTC string to input datetime-local string (YYYY-MM-DDTHH:mm)
const toLocalDatetimeInputValue = (isoStr?: string | null): string => {
  if (!isoStr) return '';
  const date = new Date(isoStr);
  if (isNaN(date.getTime())) return '';
  const pad = (n: number) => n.toString().padStart(2, '0');
  const year = date.getFullYear();
  const month = pad(date.getMonth() + 1);
  const day = date.getDate();
  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  return `${year}-${month}-${day}T${hours}:${minutes}`;
};

export const InterviewFormModal: React.FC<InterviewFormModalProps> = ({
  isOpen,
  onClose,
  applicationId,
  interviewToEdit,
  onSuccess,
}) => {
  const isEditing = Boolean(interviewToEdit);

  const createMutation = useCreateInterview();
  const updateMutation = useUpdateInterview();
  const activeMutation = isEditing ? updateMutation : createMutation;

  const [type, setType] = useState<InterviewType>('TechnicalInterview');
  const [scheduledAt, setScheduledAt] = useState('');
  const [conductedAt, setConductedAt] = useState('');
  const [notes, setNotes] = useState('');
  const [feedback, setFeedback] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (interviewToEdit) {
      setType(interviewToEdit.type);
      setScheduledAt(toLocalDatetimeInputValue(interviewToEdit.scheduledAt));
      setConductedAt(toLocalDatetimeInputValue(interviewToEdit.conductedAt));
      setNotes(interviewToEdit.notes ?? '');
      setFeedback(interviewToEdit.feedback ?? '');
    } else {
      setType('TechnicalInterview');
      setScheduledAt('');
      setConductedAt('');
      setNotes('');
      setFeedback('');
    }
    setValidationError(null);
    createMutation.reset();
    updateMutation.reset();
  }, [interviewToEdit, isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    // Backend domain requirement: Interview must have either scheduled or conducted date
    if (!scheduledAt && !conductedAt) {
      setValidationError('Interview must have either a scheduled or conducted date.');
      return;
    }

    const scheduledUtc = scheduledAt ? new Date(scheduledAt).toISOString() : null;
    const conductedUtc = conductedAt ? new Date(conductedAt).toISOString() : null;

    try {
      if (isEditing && interviewToEdit) {
        const updateDto: UpdateInterviewDto = {
          type,
          scheduledAt: scheduledUtc,
          conductedAt: conductedUtc,
          notes: notes.trim() || null,
          feedback: feedback.trim() || null,
        };

        await updateMutation.mutateAsync({
          applicationId,
          interviewId: interviewToEdit.id,
          dto: updateDto,
        });

        onSuccess?.('updated');
      } else {
        const createDto: CreateInterviewDto = {
          type,
          scheduledAt: scheduledUtc,
          conductedAt: conductedUtc,
          notes: notes.trim() || null,
          feedback: feedback.trim() || null,
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

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Interview Round' : 'Schedule Interview Round'}
      description="Track technical interviews, behavioral screenings, feedback, and discussion notes."
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {validationError && (
          <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
            {validationError}
          </div>
        )}

        {activeMutation.isError && <ErrorAlert error={activeMutation.error} />}

        {/* Interview Type */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-foreground">
            Interview Type <span className="text-destructive">*</span>
          </label>
          <select
            value={type}
            onChange={(e) => setType(e.target.value as InterviewType)}
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
          >
            {INTERVIEW_TYPE_OPTIONS.map((opt) => (
              <option key={opt} value={opt}>
                {INTERVIEW_TYPE_CONFIG[opt]?.label ?? opt}
              </option>
            ))}
          </select>
        </div>

        {/* Dates Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {/* Scheduled At */}
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <Calendar className="h-3.5 w-3.5 text-primary" />
              <span>Scheduled Date & Time</span>
            </label>
            <input
              type="datetime-local"
              value={scheduledAt}
              onChange={(e) => setScheduledAt(e.target.value)}
              className="w-full rounded-lg border border-border bg-background px-3 py-1.5 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <p className="text-[11px] text-muted-foreground">When the round is planned.</p>
          </div>

          {/* Conducted At */}
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <CheckCircle2 className="h-3.5 w-3.5 text-emerald-400" />
              <span>Conducted Date & Time</span>
            </label>
            <input
              type="datetime-local"
              value={conductedAt}
              onChange={(e) => setConductedAt(e.target.value)}
              className="w-full rounded-lg border border-border bg-background px-3 py-1.5 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <p className="text-[11px] text-muted-foreground">Fill in when the round has taken place.</p>
          </div>
        </div>

        <p className="text-[11px] text-muted-foreground italic">
          * At least one date (Scheduled or Conducted) is required per backend business rules.
        </p>

        {/* Notes */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
            <FileText className="h-3.5 w-3.5 text-primary" />
            <span>Preparation Notes & Topics</span>
          </label>
          <textarea
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            rows={3}
            placeholder="Interviewer names, meeting link, topics to prepare, coding problems mentioned..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        {/* Feedback */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
            <MessageSquare className="h-3.5 w-3.5 text-primary" />
            <span>Interview Feedback & Outcome</span>
          </label>
          <textarea
            value={feedback}
            onChange={(e) => setFeedback(e.target.value)}
            rows={3}
            placeholder="Feedback received, questions asked, difficulty assessment, areas for improvement..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
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
            {isEditing ? 'Save Changes' : 'Schedule Round'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default InterviewFormModal;
