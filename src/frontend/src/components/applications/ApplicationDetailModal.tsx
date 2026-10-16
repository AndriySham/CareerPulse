import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Modal } from '@/components/ui/Modal';
import { ErrorAlert } from '@/components/ui/ErrorAlert';
import ApplicationStatusBadge, { STATUS_CONFIG } from './ApplicationStatusBadge';
import { useApplication, useChangeApplicationStatus } from '@/api/applications';
import { useResumeRevision } from '@/api/resumes';
import InterviewSection from './InterviewSection';
import CommunicationSection from './CommunicationSection';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import {
  Building2,
  Briefcase,
  Globe,
  Calendar,
  FileText,
  ArrowRight,
  Send,
  Lock,
  ExternalLink,
  MessageSquare,
  Info,
} from 'lucide-react';

interface ApplicationDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  application: ApplicationDto | null;
}

type TabType = 'overview' | 'interviews' | 'communications';

export const ApplicationDetailModal: React.FC<ApplicationDetailModalProps> = ({
  isOpen,
  onClose,
  application,
}) => {
  const [activeTab, setActiveTab] = useState<TabType>('overview');
  const [selectedNextStatus, setSelectedNextStatus] = useState<ApplicationStatus | null>(null);
  const [transitionNotes, setTransitionNotes] = useState('');

  // Fetch full details (includes interviews, communications, recruiter)
  const { data: detailedApp } = useApplication(application?.id);
  const activeApp = detailedApp ?? application;


  const changeStatusMutation = useChangeApplicationStatus();
  const { data: linkedRevision } = useResumeRevision(activeApp?.resumeRevisionId);

  if (!activeApp) return null;

  const interviews = detailedApp?.interviews ?? [];
  const communications = detailedApp?.communications ?? [];

  const handleStatusTransition = (newStatus: ApplicationStatus) => {
    changeStatusMutation.mutate(
      {
        id: activeApp.id,
        dto: {
          newStatus,
          notes: transitionNotes.trim() || undefined,
        },
      },
      {
        onSuccess: () => {
          setSelectedNextStatus(null);
          setTransitionNotes('');
        },
      }
    );
  };

  const formattedAppliedDate = activeApp.appliedAt
    ? new Date(activeApp.appliedAt).toLocaleString(undefined, {
        dateStyle: 'medium',
        timeStyle: 'short',
      })
    : null;

  const formattedCreatedDate = new Date(activeApp.createdAt).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  });

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={activeApp.companyName}
      description="Job Application Details, Interview Pipeline & Communication Log"
      maxWidth="2xl"
    >
      <div className="space-y-5">
        {changeStatusMutation.isError && (
          <ErrorAlert error={changeStatusMutation.error} />
        )}

        {/* Top Info Banner */}
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-border/60 bg-accent/30 p-4">
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <Building2 className="h-4 w-4 text-primary" />
              <span className="font-bold text-base text-foreground">
                {activeApp.companyName}
              </span>
            </div>
            {activeApp.vacancyTitle ? (
              <div className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                <Briefcase className="h-3.5 w-3.5 text-primary/70" />
                <span>{activeApp.vacancyTitle}</span>
              </div>
            ) : (
              <span className="text-xs text-muted-foreground/60 italic">
                General Job Search Application
              </span>
            )}
          </div>

          <div className="flex flex-col items-end gap-1">
            <ApplicationStatusBadge status={activeApp.status} size="lg" />
            <span className="text-[11px] text-muted-foreground">
              Source: <strong className="text-foreground">{activeApp.jobSource}</strong>
            </span>
          </div>
        </div>

        {/* Tab Navigation */}
        <div className="flex items-center gap-1 border-b border-border/50 pb-2">
          <button
            type="button"
            onClick={() => setActiveTab('overview')}
            className={`flex items-center gap-2 px-3 py-1.5 text-xs font-semibold rounded-lg transition-all cursor-pointer ${
              activeTab === 'overview'
                ? 'bg-primary text-primary-foreground shadow-sm'
                : 'text-muted-foreground hover:text-foreground hover:bg-accent/60'
            }`}
          >
            <Info className="h-3.5 w-3.5" />
            <span>Overview</span>
          </button>

          <button
            type="button"
            onClick={() => setActiveTab('interviews')}
            className={`flex items-center gap-2 px-3 py-1.5 text-xs font-semibold rounded-lg transition-all cursor-pointer ${
              activeTab === 'interviews'
                ? 'bg-primary text-primary-foreground shadow-sm'
                : 'text-muted-foreground hover:text-foreground hover:bg-accent/60'
            }`}
          >
            <Calendar className="h-3.5 w-3.5" />
            <span>Interviews</span>
            <span
              className={`rounded-full px-1.5 py-0.2 text-[10px] font-bold ${
                activeTab === 'interviews'
                  ? 'bg-primary-foreground text-primary'
                  : 'bg-accent text-foreground'
              }`}
            >
              {interviews.length}
            </span>
          </button>

          <button
            type="button"
            onClick={() => setActiveTab('communications')}
            className={`flex items-center gap-2 px-3 py-1.5 text-xs font-semibold rounded-lg transition-all cursor-pointer ${
              activeTab === 'communications'
                ? 'bg-primary text-primary-foreground shadow-sm'
                : 'text-muted-foreground hover:text-foreground hover:bg-accent/60'
            }`}
          >
            <MessageSquare className="h-3.5 w-3.5" />
            <span>Communications</span>
            <span
              className={`rounded-full px-1.5 py-0.2 text-[10px] font-bold ${
                activeTab === 'communications'
                  ? 'bg-primary-foreground text-primary'
                  : 'bg-accent text-foreground'
              }`}
            >
              {communications.length}
            </span>
          </button>
        </div>

        {/* Tab 1: Overview */}
        {activeTab === 'overview' && (
          <div className="space-y-5 animate-in fade-in duration-150">
            {/* Grid Details */}
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 text-xs">
              <div className="rounded-xl border border-border/60 bg-card p-3.5 space-y-2">
                <div className="font-bold text-foreground flex items-center gap-1.5">
                  <Calendar className="h-3.5 w-3.5 text-primary" /> Timeline & Audit
                </div>
                <div className="text-muted-foreground space-y-1">
                  <div>
                    Created: <strong className="text-foreground">{formattedCreatedDate}</strong>
                  </div>
                  {formattedAppliedDate && (
                    <div>
                      Applied: <strong className="text-foreground">{formattedAppliedDate}</strong>
                    </div>
                  )}
                  <div>
                    Last Updated:{' '}
                    <strong className="text-foreground">
                      {new Date(activeApp.updatedAt).toLocaleDateString()}
                    </strong>
                  </div>
                </div>
              </div>

              <div className="rounded-xl border border-border/60 bg-card p-3.5 space-y-2">
                <div className="font-bold text-foreground flex items-center gap-1.5 justify-between">
                  <span className="flex items-center gap-1.5">
                    <FileText className="h-3.5 w-3.5 text-primary" /> Linked Resume Revision
                  </span>
                  <Link
                    to="/resumes"
                    onClick={onClose}
                    className="text-[11px] font-semibold text-primary hover:underline flex items-center gap-0.5"
                  >
                    Resumes <ExternalLink className="h-3 w-3" />
                  </Link>
                </div>
                <div className="text-muted-foreground space-y-1">
                  {linkedRevision ? (
                    <>
                      <div>
                        Version: <strong className="text-foreground">v{linkedRevision.version}</strong>{' '}
                        <span className="text-[11px] opacity-75">({linkedRevision.status})</span>
                      </div>
                      {linkedRevision.personalInfo?.fullName && (
                        <div className="truncate">
                          Candidate:{' '}
                          <strong className="text-foreground">
                            {linkedRevision.personalInfo.fullName}
                          </strong>
                        </div>
                      )}
                    </>
                  ) : (
                    <div>
                      Revision ID:{' '}
                      <code className="text-primary font-mono text-[11px]">
                        {activeApp.resumeRevisionId.slice(0, 8)}...
                      </code>
                    </div>
                  )}
                  <div className="flex items-center gap-1.5 text-emerald-400 font-semibold text-[11px] pt-1">
                    <Lock className="h-3 w-3" /> Immutability Locked on Submission (ADR 005)
                  </div>
                </div>
              </div>
            </div>

            {/* Quick Activity Summary widgets */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <div
                onClick={() => setActiveTab('interviews')}
                className="group rounded-xl border border-border/60 bg-card/60 p-3.5 flex items-center justify-between hover:border-primary/50 transition-all cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-lg bg-primary/10 text-primary">
                    <Calendar className="h-4 w-4" />
                  </div>
                  <div>
                    <div className="text-xs font-bold text-foreground">
                      {interviews.length} Interview Round{interviews.length === 1 ? '' : 's'}
                    </div>
                    <div className="text-[11px] text-muted-foreground">
                      {interviews.length > 0
                        ? 'Click to view scheduled rounds'
                        : 'No interviews scheduled yet'}
                    </div>
                  </div>
                </div>
                <ArrowRight className="h-4 w-4 text-muted-foreground group-hover:text-primary transition-colors" />
              </div>

              <div
                onClick={() => setActiveTab('communications')}
                className="group rounded-xl border border-border/60 bg-card/60 p-3.5 flex items-center justify-between hover:border-primary/50 transition-all cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-lg bg-primary/10 text-primary">
                    <MessageSquare className="h-4 w-4" />
                  </div>
                  <div>
                    <div className="text-xs font-bold text-foreground">
                      {communications.length} Communication Log{communications.length === 1 ? '' : 's'}
                    </div>
                    <div className="text-[11px] text-muted-foreground">
                      {communications.length > 0
                        ? 'Click to view correspondence'
                        : 'No communications logged yet'}
                    </div>
                  </div>
                </div>
                <ArrowRight className="h-4 w-4 text-muted-foreground group-hover:text-primary transition-colors" />
              </div>
            </div>

            {/* Application Notes */}
            <div className="rounded-xl border border-border/60 bg-card p-4 space-y-2">
              <div className="font-bold text-xs text-foreground flex items-center gap-1.5">
                <Globe className="h-3.5 w-3.5 text-primary" /> Application Notes
              </div>
              {activeApp.notes ? (
                <p className="text-xs text-muted-foreground leading-relaxed whitespace-pre-wrap">
                  {activeApp.notes}
                </p>
              ) : (
                <p className="text-xs text-muted-foreground/50 italic">No notes attached.</p>
              )}
            </div>

            {/* Allowed Status Transitions Panel */}
            <div className="rounded-xl border border-primary/30 bg-primary/5 p-4 space-y-3">
              <div className="flex items-center justify-between">
                <h4 className="font-bold text-xs tracking-tight text-foreground flex items-center gap-1.5">
                  <Send className="h-3.5 w-3.5 text-primary" /> Transition Application Status
                </h4>
                <span className="text-[11px] text-muted-foreground">
                  Domain State Machine Enforced
                </span>
              </div>

              {activeApp.allowedTransitions && activeApp.allowedTransitions.length > 0 ? (
                <div className="space-y-3">
                  <div className="flex flex-wrap gap-2">
                    {activeApp.allowedTransitions.map((nextStatus) => {
                      const statusInfo = STATUS_CONFIG[nextStatus];
                      const isSelected = selectedNextStatus === nextStatus;
                      return (
                        <button
                          key={nextStatus}
                          type="button"
                          onClick={() => {
                            if (isSelected) {
                              setSelectedNextStatus(null);
                            } else {
                              setSelectedNextStatus(nextStatus);
                            }
                          }}
                          className={`inline-flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-semibold transition-all cursor-pointer ${
                            isSelected
                              ? 'bg-primary text-primary-foreground border-primary shadow-sm'
                              : 'bg-card text-foreground border-border hover:border-primary/50'
                          }`}
                        >
                          <ArrowRight className="h-3 w-3" />
                          Move to {statusInfo?.label ?? nextStatus}
                        </button>
                      );
                    })}
                  </div>

                  {selectedNextStatus && (
                    <div className="rounded-lg border border-border bg-card p-3 space-y-2 animate-in fade-in duration-150">
                      <div className="text-xs font-semibold text-foreground">
                        Confirm Transition to &quot;{STATUS_CONFIG[selectedNextStatus]?.label}&quot;
                      </div>
                      <textarea
                        value={transitionNotes}
                        onChange={(e) => setTransitionNotes(e.target.value)}
                        rows={2}
                        placeholder="Add transition notes (e.g., HR feedback, interview scheduled date)..."
                        className="w-full rounded-lg border border-border bg-background px-2.5 py-1.5 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
                      />
                      <div className="flex items-center justify-end gap-2 pt-1">
                        <button
                          type="button"
                          onClick={() => setSelectedNextStatus(null)}
                          className="px-3 py-1 text-xs text-muted-foreground hover:text-foreground cursor-pointer"
                        >
                          Cancel
                        </button>
                        <button
                          type="button"
                          onClick={() => handleStatusTransition(selectedNextStatus)}
                          disabled={changeStatusMutation.isPending}
                          className="rounded-lg bg-primary px-3 py-1 text-xs font-semibold text-primary-foreground hover:bg-primary/90 disabled:opacity-50 cursor-pointer"
                        >
                          {changeStatusMutation.isPending ? 'Updating...' : 'Confirm Transition'}
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              ) : (
                <p className="text-xs text-muted-foreground/70 italic">
                  This application is in a terminal status ({activeApp.status}). No further state transitions are allowed.
                </p>
              )}
            </div>
          </div>
        )}

        {/* Tab 2: Interviews */}
        {activeTab === 'interviews' && (
          <div className="animate-in fade-in duration-150">
            <InterviewSection
              applicationId={activeApp.id}
              interviews={interviews}
            />
          </div>
        )}

        {/* Tab 3: Communications */}
        {activeTab === 'communications' && (
          <div className="animate-in fade-in duration-150">
            <CommunicationSection
              applicationId={activeApp.id}
              communications={communications}
            />
          </div>
        )}

        {/* Footer actions */}
        <div className="flex justify-end pt-2 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            className="rounded-xl bg-accent px-4 py-2 text-xs font-semibold text-foreground hover:bg-accent/80 transition-colors cursor-pointer"
          >
            Close
          </button>
        </div>
      </div>
    </Modal>
  );
};

export default ApplicationDetailModal;
