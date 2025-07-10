using System;

namespace MA_GA.domain.geneticalgorithm.objective;

public class ObjectiveSetup
{

    private int numberOfElementsPerModule;

    private bool isUseWeightSumMethod;

    private List<Objective> objectives;

    public override string ToString()
    {
        string objectiveString = "";
        if (this.objectives != null && this.objectives.Count > 0)
        {
            objectiveString = string.Join(", ", this.objectives.Select(o => o.ToString()));
        }
        return $"ObjectiveSetup: [NumberOfElementsPerModule: {this.numberOfElementsPerModule}, IsUseWeightSumMethod: {this.isUseWeightSumMethod}, Objectives: {objectiveString}]";
    }

}
