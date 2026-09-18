import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateEducation, useUpdateEducation, useEducation } from '@/api/educations';
import type { EducationDto } from '@/types';
import { Loader2 } from 'lucide-react';

interface EducationFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  resumeRevisionId: string;
  educationToEdit?: EducationDto | null;
  educationId?: string | null;
  onSuccess?: (action: 'created' | 'updated') => void;
}

export const EducationFormModal: React.FC<EducationFormModalProps> = ({
  isOpen,
  onClose,
  resumeRevisionId,
  educationToEdit,
  educationId,
  onSuccess,
}) => {
  const { data: fetchedEducation, isLoading: isLoadingSingle } = useEducation(
    educationId || undefined
  );

  const activeEducation = educationToEdit ?? fetchedEducation;
  const isEditing = Boolean(educationToEdit || educationId);

  const createMutation = useCreateEducation();
  const updateMutation = useUpdateEducation();

  const [institutionName, setInstitutionName] = useState('');
  const [description, setDescription] = useState('');
  const [startYear, setStartYear] = useState<string>('');
  const [endYear, setEndYear] = useState<string>('');
  const [validationError, setValidationError] = useState<string | null>(null);

  const currentYear = new Date().getFullYear();
  const maxEndYear = currentYear + 10;

  useEffect(() => {
    if (activeEducation) {
      setInstitutionName(activeEducation.institutionName || '');
      setDescription(activeEducation.description || '');
      setStartYear(activeEducation.startYear != null ? String(activeEducation.startYear) : '');
      setEndYear(activeEducation.endYear != null ? String(activeEducation.endYear) : '');
    } else {
      setInstitutionName('');
      setDescription('');
      setStartYear('');
      setEndYear('');
    }
    setValidationError(null);
    createMutation.reset();
    updateMutation.reset();
  }, [educationToEdit, isOpen]);

  const activeMutation = isEditing ? updateMutation : createMutation;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const trimmedInstitution = institutionName.trim();
    if (!trimmedInstitution) {
      setValidationError('Institution Name is required.');
      return;
    }

    if (trimmedInstitution.length > 300) {
      setValidationError('Institution Name must not exceed 300 characters.');
      return;
    }

    const trimmedDescription = description.trim();
    if (trimmedDescription.length > 200) {
      setValidationError('Description must not exceed 200 characters.');
      return;
    }

    const parsedStartYear = startYear !== '' ? Number.parseInt(startYear, 10) : null;
    const parsedEndYear = endYear !== '' ? Number.parseInt(endYear, 10) : null;

    if (parsedStartYear !== null) {
      if (Number.isNaN(parsedStartYear) || parsedStartYear > currentYear) {
        setValidationError(`Start year cannot be greater than the current year (${currentYear}).`);
        return;
      }
    }

    if (parsedEndYear !== null) {
      if (Number.isNaN(parsedEndYear) || parsedEndYear > maxEndYear) {
        setValidationError(`End year cannot be greater than ${maxEndYear}.`);
        return;
      }
    }

    if (parsedStartYear !== null && parsedEndYear !== null && parsedEndYear < parsedStartYear) {
      setValidationError('End year cannot be less than start year.');
      return;
    }

    const targetId = activeEducation?.id || educationId;

    try {
      if (isEditing && targetId) {
        await updateMutation.mutateAsync({
          id: targetId,
          dto: {
            resumeRevisionId,
            institutionName: trimmedInstitution,
            description: trimmedDescription || null,
            startYear: parsedStartYear,
            endYear: parsedEndYear,
          },
        });
        onSuccess?.('updated');
      } else {
        await createMutation.mutateAsync({
          resumeRevisionId,
          institutionName: trimmedInstitution,
          description: trimmedDescription || null,
          startYear: parsedStartYear,
          endYear: parsedEndYear,
        });
        onSuccess?.('created');
      }
      onClose();
    } catch {
      // Error handled by activeMutation.error
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Education' : 'Add Education'}
      description={
        isEditing
          ? 'Update academic credential details for this resume revision snapshot.'
          : 'Add university degree, college diploma, or academic credential to this resume revision.'
      }
      maxWidth="md"
    >
      {isLoadingSingle && !activeEducation ? (
        <div className="py-8 flex flex-col items-center justify-center gap-3 text-muted-foreground text-sm">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
          <span>Loading education details...</span>
        </div>
      ) : (
        <form onSubmit={handleSubmit} autoComplete="off" className="space-y-4">
          {validationError && (
            <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
              {validationError}
            </div>
          )}

          {activeMutation.error && <ErrorAlert error={activeMutation.error} />}

          {/* Institution Name */}
          <div>
            <label
              htmlFor="education-institution"
              className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
            >
              Institution Name <span className="text-destructive">*</span>
            </label>
            <input
              id="education-institution"
              type="text"
              required
              maxLength={300}
              value={institutionName}
              onChange={(e) => setInstitutionName(e.target.value)}
              placeholder="e.g. Stanford University, Dnipro National University"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <p className="text-[11px] text-muted-foreground mt-1">
              University, college, academy, or educational institution name (max 300 characters).
            </p>
          </div>

          {/* Description */}
          <div>
            <div className="flex items-center justify-between mb-1">
              <label
                htmlFor="education-description"
                className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
              >
                Description / Degree / Field of Study
              </label>
              <span
                className={`text-[10px] ${
                  description.length > 200 ? 'text-destructive font-bold' : 'text-muted-foreground'
                }`}
              >
                {description.length}/200
              </span>
            </div>
            <input
              id="education-description"
              type="text"
              maxLength={200}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="e.g. Master's Degree in Software Engineering, Bachelor of Computer Science"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <p className="text-[11px] text-muted-foreground mt-1">
              Degree title, major, academic achievements, or honors (max 200 characters).
            </p>
          </div>

          {/* Start Year & End Year */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label
                htmlFor="education-start-year"
                className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
              >
                Start Year
              </label>
              <input
                id="education-start-year"
                type="number"
                min={1950}
                max={currentYear}
                value={startYear}
                onChange={(e) => setStartYear(e.target.value)}
                placeholder={`e.g. ${currentYear - 4}`}
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
              <p className="text-[10px] text-muted-foreground mt-1">
                Year started (cannot exceed {currentYear}).
              </p>
            </div>

            <div>
              <label
                htmlFor="education-end-year"
                className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
              >
                End Year (or Expected)
              </label>
              <input
                id="education-end-year"
                type="number"
                min={startYear ? Number.parseInt(startYear, 10) : 1950}
                max={maxEndYear}
                value={endYear}
                onChange={(e) => setEndYear(e.target.value)}
                placeholder={`e.g. ${currentYear}`}
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
              <p className="text-[10px] text-muted-foreground mt-1">
                Graduation year (cannot exceed {maxEndYear}).
              </p>
            </div>
          </div>

          {/* Actions Footer */}
          <div className="flex items-center justify-end gap-3 pt-4 border-t border-border/40">
            <button
              type="button"
              onClick={onClose}
              disabled={activeMutation.isPending}
              className="rounded-lg px-4 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50 cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={activeMutation.isPending || !institutionName.trim()}
              className="inline-flex items-center justify-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-colors disabled:opacity-50 cursor-pointer"
            >
              {activeMutation.isPending && <Loader2 className="h-4 w-4 animate-spin" />}
              <span>
                {activeMutation.isPending
                  ? isEditing
                    ? 'Saving...'
                    : 'Adding...'
                  : isEditing
                  ? 'Save Changes'
                  : 'Add Education'}
              </span>
            </button>
          </div>
        </form>
      )}
    </Modal>
  );
};

export default EducationFormModal;
