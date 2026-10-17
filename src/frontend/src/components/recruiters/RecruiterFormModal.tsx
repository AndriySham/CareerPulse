import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateRecruiter, useUpdateRecruiter } from '@/api/recruiters';
import type { RecruiterDto, CreateRecruiterDto, UpdateRecruiterDto } from '@/types';
import { UserCheck, Mail, Phone, Send, Linkedin, FileText, Loader2 } from 'lucide-react';

interface RecruiterFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  companyId: string;
  recruiterToEdit?: RecruiterDto | null;
  onSuccess?: (action: 'created' | 'updated', recruiter?: RecruiterDto) => void;
}

export const RecruiterFormModal: React.FC<RecruiterFormModalProps> = ({
  isOpen,
  onClose,
  companyId,
  recruiterToEdit,
  onSuccess,
}) => {
  const isEditing = Boolean(recruiterToEdit);

  const createMutation = useCreateRecruiter();
  const updateMutation = useUpdateRecruiter();
  const activeMutation = isEditing ? updateMutation : createMutation;

  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [linkedInUrl, setLinkedInUrl] = useState('');
  const [telegramUrl, setTelegramUrl] = useState('');
  const [notes, setNotes] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (recruiterToEdit) {
      setName(recruiterToEdit.name ?? '');
      setPhone(recruiterToEdit.phone ?? '');
      setEmail(recruiterToEdit.email ?? '');
      setLinkedInUrl(recruiterToEdit.linkedInUrl ?? '');
      setTelegramUrl(recruiterToEdit.telegramUrl ?? '');
      setNotes(recruiterToEdit.notes ?? '');
    } else {
      setName('');
      setPhone('');
      setEmail('');
      setLinkedInUrl('');
      setTelegramUrl('');
      setNotes('');
    }
    setValidationError(null);
    createMutation.reset();
    updateMutation.reset();
  }, [recruiterToEdit, isOpen]);

  const normalizeUrl = (url: string, defaultHost?: string): string => {
    const trimmed = url.trim();
    if (!trimmed) return '';
    if (trimmed.startsWith('http://') || trimmed.startsWith('https://')) {
      return trimmed;
    }
    if (defaultHost && !trimmed.includes('/')) {
      return `https://${defaultHost}/${trimmed.replace(/^@/, '')}`;
    }
    return `https://${trimmed}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const trimmedName = name.trim();
    if (!trimmedName) {
      setValidationError('Recruiter Name is required.');
      return;
    }
    if (trimmedName.length > 100) {
      setValidationError('Recruiter Name must not exceed 100 characters.');
      return;
    }

    const trimmedEmail = email.trim();
    if (trimmedEmail && trimmedEmail.length > 100) {
      setValidationError('Email must not exceed 100 characters.');
      return;
    }
    if (trimmedEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmedEmail)) {
      setValidationError('Please enter a valid email address.');
      return;
    }

    const trimmedPhone = phone.trim();
    if (trimmedPhone && trimmedPhone.length > 20) {
      setValidationError('Phone must not exceed 20 characters.');
      return;
    }

    let normalizedLinkedIn = linkedInUrl.trim();
    if (normalizedLinkedIn) {
      normalizedLinkedIn = normalizeUrl(normalizedLinkedIn);
      if (normalizedLinkedIn.length > 500) {
        setValidationError('LinkedIn URL must not exceed 500 characters.');
        return;
      }
    }

    let normalizedTelegram = telegramUrl.trim();
    if (normalizedTelegram) {
      normalizedTelegram = normalizeUrl(normalizedTelegram, 't.me');
      if (normalizedTelegram.length > 500) {
        setValidationError('Telegram URL must not exceed 500 characters.');
        return;
      }
    }

    const trimmedNotes = notes.trim();
    if (trimmedNotes && trimmedNotes.length > 4000) {
      setValidationError('Notes must not exceed 4000 characters.');
      return;
    }

    try {
      if (isEditing && recruiterToEdit) {
        const updateDto: UpdateRecruiterDto = {
          name: trimmedName,
          phone: trimmedPhone || null,
          email: trimmedEmail || null,
          linkedInUrl: normalizedLinkedIn || null,
          telegramUrl: normalizedTelegram || null,
          notes: trimmedNotes || null,
        };

        const updated = await updateMutation.mutateAsync({
          companyId,
          recruiterId: recruiterToEdit.id,
          dto: updateDto,
        });

        onSuccess?.('updated', updated);
      } else {
        const createDto: CreateRecruiterDto = {
          name: trimmedName,
          phone: trimmedPhone || null,
          email: trimmedEmail || null,
          linkedInUrl: normalizedLinkedIn || null,
          telegramUrl: normalizedTelegram || null,
          notes: trimmedNotes || null,
        };

        const created = await createMutation.mutateAsync({
          companyId,
          dto: createDto,
        });

        onSuccess?.('created', created);
      }

      onClose();
    } catch {
      // Handled by activeMutation.error and ErrorAlert
    }
  };

  const remainingChars = 4000 - notes.length;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Recruiter' : 'Add Recruiter'}
      description="Manage contact channels and background notes for this company recruiter."
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {validationError && (
          <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
            {validationError}
          </div>
        )}

        {activeMutation.isError && <ErrorAlert error={activeMutation.error} />}

        {/* Name */}
        <div className="space-y-1.5">
          <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
            <UserCheck className="h-3.5 w-3.5 text-primary" />
            <span>Recruiter Name <span className="text-destructive">*</span></span>
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. Alex Morgan"
            maxLength={100}
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            required
          />
        </div>

        {/* Email & Phone */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <Mail className="h-3.5 w-3.5 text-primary" />
              <span>Email</span>
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="alex.morgan@company.com"
              maxLength={100}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <Phone className="h-3.5 w-3.5 text-primary" />
              <span>Phone</span>
            </label>
            <input
              type="tel"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="+1 555 123 4567"
              maxLength={20}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        {/* Telegram & LinkedIn */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <Send className="h-3.5 w-3.5 text-sky-400" />
              <span>Telegram URL or Handle</span>
            </label>
            <input
              type="text"
              value={telegramUrl}
              onChange={(e) => setTelegramUrl(e.target.value)}
              placeholder="@alex_recruiter or https://t.me/alex"
              maxLength={500}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <Linkedin className="h-3.5 w-3.5 text-[#0A66C2]" />
              <span>LinkedIn Profile URL</span>
            </label>
            <input
              type="text"
              value={linkedInUrl}
              onChange={(e) => setLinkedInUrl(e.target.value)}
              placeholder="https://linkedin.com/in/alexmorgan"
              maxLength={500}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        {/* Notes */}
        <div className="space-y-1.5">
          <div className="flex items-center justify-between">
            <label className="text-xs font-semibold text-foreground flex items-center gap-1.5">
              <FileText className="h-3.5 w-3.5 text-primary" />
              <span>Notes & Background</span>
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
              {notes.length} / 4000
            </span>
          </div>
          <textarea
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            rows={4}
            maxLength={4100}
            placeholder="Recruiter preferences, responsiveness, interview stages feedback, personal notes..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary leading-relaxed"
          />
        </div>

        {/* Form Actions */}
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
            {isEditing ? 'Save Changes' : 'Create Recruiter'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default RecruiterFormModal;
