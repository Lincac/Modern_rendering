#include"Mesh_Simplify.h"

int main(int argc, char** argv) {
    Mesh_Simplify model;
    if (argc != 4) {
        printf("Usage:\n ./main [Input Object] [Output Object] [Simplify Rate] [Threshold Value]");
        return 0;
    }
    std::string inputModelFileName(argv[1]);
    std::string outputModelFileName(argv[2]);
    double simplifyRate = atof(argv[3]);


    printf("inputModelFileName: %s\n", inputModelFileName.c_str());
    printf("outputModelFileName: %s\n", outputModelFileName.c_str());
    printf("simplifyRate: %.4lf\n", simplifyRate);
    printf("threshold: %.4lf\n", INFD);
    printf("------------------------------------\n");

    time_t start = time(0);

    model.loadFromFile(inputModelFileName);

    size_t all = model.getFaceN();
    size_t simple = all * simplifyRate;

    printf("vertex: %zu\n", model.getVertexN());
    printf("edge: %zu\n", model.getEdgeN());
    printf("simple / all = %zu / %zu\n", simple, all);
    model.simplify(simple, INFD);

    model.saveToFile(outputModelFileName);
    time_t end = time(0);
    printf("%cSave to [%s] successfully. Time %ld sec.\n", 13, outputModelFileName.c_str(), end - start);
    return 0;
}