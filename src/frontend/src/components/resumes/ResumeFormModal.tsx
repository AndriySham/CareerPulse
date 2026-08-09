import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import CustomSelect from '@/components/ui/CustomSelect';
import { useCreateResumeDraft, useUpdateResumeDraft } from '@/api/resumes';
import { useMasterSkills } from '@/api/masterSkills';
import type {
  ResumeRevisionDto,
  PersonalInfo,
  ResumeSkillInputDto,
  CreateResumeDraftDto,
  UpdateResumeDraftDto,
} from '@/types';
import { Plus, Trash2, Award, User, FileText, Loader2 } from 'lucide-react';

interface ResumeFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  revisionToEdit?: ResumeRevisionDto | null;
}

export const ResumeFormModal: React.FC<ResumeFormModalProps> = ({
  isOpen,
  onClose,
  revisionToEdit,
}) => {
  const isEditing = Boolean(revisionToEdit);

  // Form State
  const [personalInfo, setPersonalInfo] = useState<PersonalInfo>({
    fullName: '',
    email: '',
    phone: '',
    linkedIn: '',
    gitHub: '',
    location: '',
  });
  const [professionalSummary, setProfessionalSummary] = useState('');
  const [attachedSkills, setAttachedSkills] = useState<ResumeSkillInputDto[]>([]);

  // Skill Selector Temporary State
  const [selectedSkillId, setSelectedSkillId] = useState('');
  const [selectedProficiency, setSelectedProficiency] = useState(3);

  // Error State
  const [formError, setFormError] = useState<unknown | null>(null);

  // API Hooks
  const createMutation = useCreateResumeDraft();
  const updateMutation = useUpdateResumeDraft();
  const { data: masterSkills = [] } = useMasterSkills();

  // Populate form when editing or resetting
  useEffect(() => {
    if (revisionToEdit) {
      setPersonalInfo({
        fullName: revisionToEdit.personalInfo?.fullName || '',
        email: revisionToEdit.personalInfo?.email || '',
        phone: revisionToEdit.personalInfo?.phone || '',
        linkedIn: revisionToEdit.personalInfo?.linkedIn || '',
        gitHub: revisionToEdit.personalInfo?.gitHub || '',
        location: revisionToEdit.personalInfo?.location || '',
      });
      setProfessionalSummary(revisionToEdit.professionalSummary || '');
      setAttachedSkills(
        revisionToEdit.skills?.map((s) => ({
          masterSkillId: s.masterSkillId,
          proficiencyLevel: s.proficiencyLevel,
        })) || []
      );
    } else {
      setPersonalInfo({
        fullName: '',
        email: '',
        phone: '',
        linkedIn: '',
        gitHub: '',
        location: '',
      });
      setProfessionalSummary('');
      setAttachedSkills([]);
    }
    setFormError(null);
    setSelectedSkillId('');
    setSelectedProficiency(3);
  }, [revisionToEdit, isOpen]);

  const handleAddSkill = () => {
    if (!selectedSkillId) return;
    if (attachedSkills.some((s) => s.masterSkillId === selectedSkillId)) {
      return; // Already added
    }
    setAttachedSkills([
      ...attachedSkills,
      { masterSkillId: selectedSkillId, proficiencyLevel: selectedProficiency },
    ]);
    setSelectedSkillId('');
    setSelectedProficiency(3);
  };

  const handleRemoveSkill = (skillId: string) => {
    setAttachedSkills(attachedSkills.filter((s) => s.masterSkillId !== skillId));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);

    const payload: CreateResumeDraftDto | UpdateResumeDraftDto = {
      personalInfo: {
        fullName: personalInfo.fullName.trim(),
        email: personalInfo.email.trim(),
        phone: personalInfo.phone?.trim() || null,
        linkedIn: personalInfo.linkedIn?.trim() || null,
        gitHub: personalInfo.gitHub?.trim() || null,
        location: personalInfo.location?.trim() || null,
      },
      professionalSummary: professionalSummary.trim(),
      skills: attachedSkills,
    };

    if (isEditing && revisionToEdit) {
      updateMutation.mutate(
        { id: revisionToEdit.id, dto: payload },
        {
          onSuccess: () => {
            onClose();
          },
          onError: (err) => {
            setFormError(err);
          },
        }
      );
    } else {
      createMutation.mutate(payload as CreateResumeDraftDto, {
        onSuccess: () => {
          onClose();
        },
        onError: (err) => {
          setFormError(err);
        },
      });
    }
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  // Unselected master skills for dropdown
  const availableSkillOptions = masterSkills
    .filter((ms) => !attachedSkills.some((as) => as.masterSkillId === ms.id))
    .map((ms) => ({
      value: ms.id,
      label: `${ms.name} (${ms.category})`,
    }));

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? `Edit Resume Draft (v${revisionToEdit?.version})` : 'Create Resume Draft'}
      description={
        isEditing
          ? 'Modify summary, contact details, or attached skills for this draft.'
          : 'Create a new Version 1 draft resume profile.'
      }
      maxWidth="xl"
    >
      <form onSubmit={handleSubmit} className="space-y-5">
        <ErrorAlert error={formError} onDismiss={() => setFormError(null)} />

        {/* Section 1: Contact Details */}
        <div className="space-y-3 rounded-lg border border-border/40 bg-accent/20 p-4">
          <div className="flex items-center gap-2 text-sm font-semibold text-foreground border-b border-border/40 pb-2">
            <User className="h-4 w-4 text-primary" />
            <span>Personal Information</span>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Full Name <span className="text-destructive">*</span>
              </label>
              <input
                type="text"
                required
                value={personalInfo.fullName}
                onChange={(e) => setPersonalInfo({ ...personalInfo, fullName: e.target.value })}
                placeholder="e.g. Alex Johnson"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Email Address <span className="text-destructive">*</span>
              </label>
              <input
                type="email"
                required
                value={personalInfo.email}
                onChange={(e) => setPersonalInfo({ ...personalInfo, email: e.target.value })}
                placeholder="e.g. alex.johnson@example.com"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Phone Number
              </label>
              <input
                type="tel"
                value={personalInfo.phone || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, phone: e.target.value })}
                placeholder="e.g. +1 (555) 234-5678"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Location
              </label>
              <input
                type="text"
                value={personalInfo.location || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, location: e.target.value })}
                placeholder="e.g. San Francisco, CA"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                LinkedIn Profile URL
              </label>
              <input
                type="url"
                value={personalInfo.linkedIn || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, linkedIn: e.target.value })}
                placeholder="https://linkedin.com/in/username"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                GitHub Profile URL
              </label>
              <input
                type="url"
                value={personalInfo.gitHub || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, gitHub: e.target.value })}
                placeholder="https://github.com/username"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>
        </div>

        {/* Section 2: Professional Summary */}
        <div className="space-y-2">
          <label className="flex items-center gap-2 text-xs font-semibold text-foreground">
            <FileText className="h-4 w-4 text-primary" />
            <span>Professional Summary</span>
            <span className="text-destructive">*</span>
          </label>
          <textarea
            required
            rows={4}
            value={professionalSummary}
            onChange={(e) => setProfessionalSummary(e.target.value)}
            placeholder="Write a compelling overview of your software engineering expertise, core technical strengths, and career highlights..."
            className="w-full rounded-lg border border-border bg-background p-3 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        {/* Section 3: Master Skills Attachment */}
        <div className="space-y-3 rounded-lg border border-border/40 bg-accent/20 p-4">
          <div className="flex items-center justify-between border-b border-border/40 pb-2">
            <div className="flex items-center gap-2 text-sm font-semibold text-foreground">
              <Award className="h-4 w-4 text-primary" />
              <span>Skills Catalog Normalization (ADR 006)</span>
            </div>
            <span className="text-xs text-muted-foreground">
              {attachedSkills.length} skill(s) attached
            </span>
          </div>

          {/* Add skill controls */}
          <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-2">
            <CustomSelect
              value={selectedSkillId}
              onChange={(val) => setSelectedSkillId(val)}
              options={availableSkillOptions}
              placeholder="Select skill from catalog..."
              className="flex-1 min-w-[200px]"
            />

            <div className="flex items-center gap-2">
              <span className="text-xs text-muted-foreground font-medium whitespace-nowrap">
                Proficiency:
              </span>
              <select
                value={selectedProficiency}
                onChange={(e) => setSelectedProficiency(Number(e.target.value))}
                className="rounded-lg border border-border bg-background px-2 py-2 text-xs font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer"
              >
                <option value={1}>1 - Beginner</option>
                <option value={2}>2 - Elementary</option>
                <option value={3}>3 - Intermediate</option>
                <option value={4}>4 - Advanced</option>
                <option value={5}>5 - Expert</option>
              </select>

              <button
                type="button"
                onClick={handleAddSkill}
                disabled={!selectedSkillId}
                className="inline-flex items-center gap-1 rounded-lg bg-primary px-3 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Plus className="h-3.5 w-3.5" /> Add
              </button>
            </div>
          </div>

          {/* Attached Skills List */}
          {attachedSkills.length > 0 ? (
            <div className="flex flex-wrap gap-2 pt-2">
              {attachedSkills.map((s) => {
                const masterSkill = masterSkills.find((ms) => ms.id === s.masterSkillId);
                const skillName = masterSkill ? masterSkill.name : s.masterSkillId;

                return (
                  <span
                    key={s.masterSkillId}
                    className="inline-flex items-center gap-2 rounded-lg bg-background border border-border/80 px-3 py-1 text-xs font-medium text-foreground shadow-xs"
                  >
                    <span>{skillName}</span>
                    <span className="rounded bg-primary/20 text-primary px-1.5 py-0.5 text-[10px] font-bold">
                      Lvl {s.proficiencyLevel}
                    </span>
                    <button
                      type="button"
                      onClick={() => handleRemoveSkill(s.masterSkillId)}
                      className="text-muted-foreground hover:text-destructive transition-colors ml-1"
                      title="Remove skill"
                    >
                      <Trash2 className="h-3.5 w-3.5" />
                    </button>
                  </span>
                );
              })}
            </div>
          ) : (
            <p className="text-xs text-muted-foreground italic text-center py-2">
              No skills attached yet. Select skills from the catalog above.
            </p>
          )}
        </div>

        {/* Action Buttons */}
        <div className="flex items-center justify-end gap-3 pt-3 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            className="rounded-lg border border-border bg-background px-4 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors"
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={isSubmitting}
            className="inline-flex items-center gap-2 rounded-lg bg-primary px-5 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all disabled:opacity-50 cursor-pointer"
          >
            {isSubmitting && <Loader2 className="h-4 w-4 animate-spin" />}
            {isEditing ? 'Save Changes' : 'Create Resume Draft'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default ResumeFormModal;
